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
	public bool IsSwimming;
	[Export] private float _bobbingSpeed = 2.0f;
	[Export] private float _bobbingAmplitude = 0.25f;
	private float _defaultBSAmplitude;
	private float _timer;
	[Export] private float _swaySpeed = 2.0f;
	[Export] private float _swayAmplitude = 2.0f;
	private float _runningSpeed;
	
	
	
	public override void _Ready() {

		_timer = 0.0f;
		_defaultBSAmplitude = 0.0f;
		_runningSpeed = _movementSpeed*_speedMultiplier;
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
		IsSwimming = false;

	}

	private void SetSensitivity(float sensitivity) {
		_mouseSensitivity=sensitivity;
	}

	public override void _Process(double delta)
	{
		_timer += (float)delta;
		if(_movementDirectionTranslation != Vector3.Zero) {
			_playerCamera.VOffset = _defaultBSAmplitude + Mathf.Sin(_timer * _bobbingSpeed)*_bobbingAmplitude;
			_playerCamera.HOffset = _defaultBSAmplitude + Mathf.Sin(_timer*_swaySpeed)*_swayAmplitude;
		} else {
			_playerCamera.VOffset= Mathf.Lerp(_playerCamera.VOffset, _defaultBSAmplitude, 0.1f);
			_playerCamera.HOffset= Mathf.Lerp(_playerCamera.HOffset, _defaultBSAmplitude, 0.3f);
			_timer = 0.0f;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		_calcVelocity = Velocity;
		if (!IsOnFloor() && !IsSwimming)
		{
			_calcVelocity.Y += _jumpFAll* (float)delta;
		}

		if (IsOnFloor()&& Input.IsActionPressed("Jump_Movement") && !IsSwimming)
		{
			_calcVelocity.Y = _jumpForce;
			
		}

		if(IsSwimming) {
			if(Input.IsActionPressed("Jump_Movement")) {
				_calcVelocity.Y = _movementSpeed;
				if(Input.IsActionPressed("Run_Movement")) {
					_calcVelocity.Y *= _speedMultiplier;
				}

			} else if(Input.IsActionPressed("Crouch_Movement")) {
				_calcVelocity.Y = _movementSpeed * -1.0f;
				if(Input.IsActionPressed("Run_Movement")) {
					_calcVelocity.Y *= _speedMultiplier;
				}
			} else {
				_calcVelocity.Y  = GetGravity().Y;
			}
		}


		_movementDirection = Input.GetVector("Left_Movement", "Right_Movement","Backward_Movement", "Forward_Movement");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		_movementDirectionTranslation = _movementDirection.X * _yawNode.Basis.X - _movementDirection.Y * _yawNode.Basis.Z;
		_movementDirectionTranslation.Y = 0;
		
		
		if (_movementDirectionTranslation != Vector3.Zero)
		{
			
			
			_calcVelocity.X = _movementDirectionTranslation.X * _movementSpeed;
			_calcVelocity.Z = _movementDirectionTranslation.Z * _movementSpeed;
			
			if (Input.IsActionPressed("Run_Movement"))
			{
				_playerCamera.VOffset = _defaultBSAmplitude + Mathf.Sin(_timer * (_bobbingSpeed+1.0f))*_bobbingAmplitude;
				_playerCamera.HOffset = _defaultBSAmplitude + Mathf.Sin(_timer*(_swaySpeed+1.0f))*_swayAmplitude;
				_calcVelocity.X = _movementDirectionTranslation.X * _runningSpeed;
				_calcVelocity.Z = _movementDirectionTranslation.Z * _runningSpeed;
				
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
			float clampedPitch = Mathf.Clamp(_pitchNode.Rotation.X - _mousePosC.Y, -1.0f, 1.0f); 
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
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
				_playerMainMenu.Show();
				GameManager.Instance.gamepaused = true;
				GetTree().Paused = true;
				
			}
			
		}
	}

	private void pauseGame() {
		
		
	}
}
