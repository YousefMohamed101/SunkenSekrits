using System;
using System.Text.RegularExpressions;
using Godot;

public partial class Player : CharacterBody3D {
	[Export] private CanvasLayer _hudLayer;
	[Export] private PackedScene _mainMenu;
	private Vector2 _mousePosC;
	private float _mouseSensitivity = 1.0f;
	[Export] private Node3D _pitchNode;
	private MainMenu _playerMainMenu;
	[Export] private Node3D _yawNode;
	[Export] public float BobbingAmplitude = 0.25f;
	[Export] public float BobbingSpeed = 2.0f;
	public Vector3 CalcVelocity;
	public Marker3D CameraAnchoring;
	public float DefaultBsAmplitude;
	[Export] public float FallSpeed = 5.0f;
	[Export] public float JumpForce = 5.0f;


	[Export] public StateMachineManager Manager;
	public Vector2 MovementDirection;
	public Vector3 MovementDirectionTranslation;
	[Export] public float MovementSpeed = 100f;
	[Export] public Camera3D PlayerCamera;
	[Export] public float SpeedMultiplier = 1.5f;
	[Export] public float SwayAmplitude = 2.0f;
	[Export] public float SwaySpeed = 2.0f;
	[Export] public float SwimmingSpeed = 5.0f;
	public float Timer;


	public override void _Ready() {
		CameraAnchoring = GetNode<Marker3D>("CameraSmoothing");
		Timer = 0.0f;
		DefaultBsAmplitude = 0.0f;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_playerMainMenu = _mainMenu.Instantiate<MainMenu>();
		_hudLayer.Layer = 2;
		_hudLayer.AddChild(_playerMainMenu);
		_playerMainMenu.Visible = false;
		_playerMainMenu.ProcessMode = ProcessModeEnum.Always;
		GameManager.Instance.MouseSenseChanged += SetSensitivity;
		GameManager.Instance.FovChanged += PlayerCamera.SetFov;
		_mouseSensitivity = SaveAndLoadManager.Instance.GetUserSetting().MouseSensitivity;
		PlayerCamera.SetFov(SaveAndLoadManager.Instance.GetUserSetting().Fov);
	}

	private void SetSensitivity(float sensitivity) { _mouseSensitivity = sensitivity; }

	public override void _Process(double delta) {}

	public override void _PhysicsProcess(double delta) {
		MovementDirection = Input.GetVector("Left_Movement", "Right_Movement", "Backward_Movement", "Forward_Movement");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		MovementDirectionTranslation = MovementDirection.X * _yawNode.Basis.X - MovementDirection.Y * _yawNode.Basis.Z;
	}

	public override void _Input(InputEvent @event) {
		if(@event is InputEventMouseMotion eventMouseMotion) {
			_mousePosC = (eventMouseMotion.Relative / 1080 * Mathf.Pi) * _mouseSensitivity;
			_yawNode.RotateY(-_mousePosC.X);
			float clampedPitch = Mathf.Clamp(_pitchNode.Rotation.X - _mousePosC.Y, -1.0f, 1.0f);
			_pitchNode.Rotation = new Vector3(clampedPitch, _pitchNode.Rotation.Y, _pitchNode.Rotation.Z);
		}
	}

	public override void _UnhandledInput(InputEvent @event) {
		if(Input.IsActionPressed("pause_game_ignore") && GameManager.Instance.gamepaused == false) {
			if(!_playerMainMenu.Visible) {
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
				_playerMainMenu.Show();
				GameManager.Instance.gamepaused = true;
				GetTree().Paused = true;
			}
		}
	}
}