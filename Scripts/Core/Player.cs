using Godot;

public partial class Player : CharacterBody3D
{
	[Export] private float _speedMultiplier = 1.5f;
	[Export] private float _movementSpeed = 5.0f;
	[Export] private float _jumpForce = 5.0f;
	[Export] private Camera3D _playerCamera;
	private Vector2 _movementDirection;
	private Vector2 _mousePosC;
	private Vector3 _movementDirectionTranslation;
	private Vector3 _calcVelocity;
	[Export] private Node3D _yawNode;
	[Export] private Node3D _pitchNode;
	[Export]private PackedScene _mainMenu;
	private CanvasLayer _hudLayer;
	private MainMenu _playerMainMenu;
	[Export]private float _jumpHeight;
	[Export]private float _jumpTP;
	[Export]private float _jumpTD;
	private float _jumpGravity;
	private float _jumpFAll;
	private float _mouseSensitivity =1.0f;

	
	
	public override void _Ready()
	{
		_jumpForce = (2 * _jumpHeight)/_jumpTP;
		_jumpGravity = (-2 * _jumpHeight)/(_jumpTP*_jumpTP);
		_jumpFAll = (-2 * _jumpHeight)/(_jumpTD*_jumpTD);
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_hudLayer = GetNode<CanvasLayer>("%HUD");
		_playerMainMenu =_mainMenu.Instantiate<MainMenu>();
		_hudLayer.AddChild(_playerMainMenu);
		_playerMainMenu.Visible = false;
		_playerMainMenu.ProcessMode = ProcessModeEnum.Always;
		GameManager.Instance.MouseSenseChanged +=  SetSensitivity;
		GameManager.Instance.FOVChanged += _playerCamera.SetFov;
		_mouseSensitivity = SaveAndLoadManager.Instance.GetUserSetting().MouseSensitivity;
		_playerCamera.SetFov(SaveAndLoadManager.Instance.GetUserSetting().Fov);


	}

	private void SetSensitivity(float sensitivity) {
		_mouseSensitivity=sensitivity;
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		_calcVelocity = Velocity;
		if (!IsOnFloor())
		{
			_calcVelocity.Y += _jumpFAll* (float)delta;
		}

		if (IsOnFloor()&& Input.IsActionPressed("Jump (Movement)"))
		{
			_calcVelocity.Y = _jumpForce;
		}
		_movementDirection = Input.GetVector("Left (Movement)", "Right (Movement)","Backward (Movement)", "Forward (Movement)");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		
		_movementDirectionTranslation = _movementDirection.X * _yawNode.Basis.X - _movementDirection.Y * _yawNode.Basis.Z;
		
		
		if (_movementDirectionTranslation != Vector3.Zero)
		{

			_calcVelocity.X = _movementDirectionTranslation.X * _movementSpeed;
			_calcVelocity.Z = _movementDirectionTranslation.Z * _movementSpeed;
			if (Input.IsActionPressed("Run (Movement)"))
			{
				_calcVelocity.X = _movementDirectionTranslation.X * _movementSpeed * _speedMultiplier;
				_calcVelocity.Z = _movementDirectionTranslation.Z * _movementSpeed * _speedMultiplier;
			}

		}
		else
		{
			_calcVelocity.X = 0.0f;
			_calcVelocity.Z = 0.0f;
		}
		
		Velocity = _calcVelocity;
		MoveAndSlide();
		
		

	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			
			_mousePosC = (eventMouseMotion.Relative/1080 * Mathf.Pi)*_mouseSensitivity;
			
			_yawNode.RotateY(-_mousePosC.X);
			//float clampedPitch = Mathf.Clamp(_pitchNode.Rotation.X - _mousePosC.Y, -Mathf.Pi / 2, Mathf.Pi / 2); 
			float clampedPitch = Mathf.Clamp(_pitchNode.Rotation.X - _mousePosC.Y, -0.8f, 0.8f); 
			_pitchNode.Rotation = new Vector3(clampedPitch, _pitchNode.Rotation.Y, _pitchNode.Rotation.Z); 
			
			
			//_mousePositionX -= eventMouseMotion.Relative.X;
			//MousePositionY -= eventMouseMotion.Relative.Y*0.5f;
			//MousePositionY = Mathf.Clamp(MousePositionY, -89f, 89f);
			//PlayerCamera.GlobalRotationDegrees = new Vector3(_pitch, _yaw, 0);
			
		}

	
		
	}

	public override void _UnhandledInput(InputEvent @event){
		if(Input.IsActionPressed("pause_game_ignore") && GameManager.Instance.gamepaused == false) {
			if(!_playerMainMenu.Visible) {
				Input.MouseMode = Input.MouseModeEnum.Visible;
				_playerMainMenu.Show();
				GameManager.Instance.gamepaused = true;
				GetTree().Paused = true;
				
			}
			
		}
	}

	private void pauseGame() {
		
		
	}
}
