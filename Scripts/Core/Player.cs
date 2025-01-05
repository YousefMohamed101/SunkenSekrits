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
	
	
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		_calcVelocity = Velocity;
		if (!IsOnFloor())
		{
			_calcVelocity += GetGravity() * (float)delta;
		}

		if (IsOnFloor()&& Input.IsActionPressed("Jump"))
		{
			_calcVelocity.Y = _jumpForce;
		}
		_movementDirection = Input.GetVector("Move_Left", "Move_Right","Move_Backward", "Move_Forward");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		
		_movementDirectionTranslation = _movementDirection.X * _yawNode.Basis.X - _movementDirection.Y * _yawNode.Basis.Z;
		
		
		if (_movementDirectionTranslation != Vector3.Zero)
		{

			_calcVelocity.X = _movementDirectionTranslation.X * _movementSpeed;
			_calcVelocity.Z = _movementDirectionTranslation.Z * _movementSpeed;
			if (Input.IsActionPressed("Run"))
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
			
			_mousePosC = (eventMouseMotion.Relative/1080 * Mathf.Pi)*0.5f;
			
			_yawNode.RotateY(-_mousePosC.X);
			float clampedPitch = Mathf.Clamp(_pitchNode.Rotation.X - _mousePosC.Y, -Mathf.Pi / 2, Mathf.Pi / 2); 
			_pitchNode.Rotation = new Vector3(clampedPitch, _pitchNode.Rotation.Y, _pitchNode.Rotation.Z); 
			
			
			//_mousePositionX -= eventMouseMotion.Relative.X;
			//MousePositionY -= eventMouseMotion.Relative.Y*0.5f;
			//MousePositionY = Mathf.Clamp(MousePositionY, -89f, 89f);
			//PlayerCamera.GlobalRotationDegrees = new Vector3(_pitch, _yaw, 0);
			
		}
	}
}
