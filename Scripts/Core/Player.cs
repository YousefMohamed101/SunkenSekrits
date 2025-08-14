using System;
using System.Text.RegularExpressions;
using Godot;

public partial class Player : CharacterBody3D {
	[Export] private CameraController _cameraController;


	[ExportGroup("Nodes")] [Export] private AudioStreamPlayer3D _footstepPlayer;

	[Export] private Hud _hudLayer;
	private string _interactText = "Press Interact";
	[Export] private PackedScene _mainMenu;
	private Vector2 _mousePosC;
	private float _mouseSensitivity = 1.0f;
	private MainMenu _playerMainMenu;
	[Export] private RayCast3D _rayCast;
	private AudioStream[] _sounds;
	[Export] public float BobbingAmplitude = 0.25f;
	[Export] public float BobbingSpeed = 2.0f;
	public Vector3 CalcVelocity;
	public Marker3D CameraAnchoring;
	private Interactable CurrentInteractable;
	public float DefaultBsAmplitude;
	[Export] public float FallSpeed = 5.0f;
	[Export] public float JumpForce = 5.0f;
	public Vector2 MousePosition;

	//runtime 
	public Vector2 MovementDirection;
	public Vector3 MovementDirectionTranslation;


	[ExportGroup("Player Stats")] [Export] public float MovementSpeed = 100f;

	[Export] public NarcosisEffect NarcosisEffect;
	[Export] public Camera3D PlayerCamera;
	[Export] public int Reach = 2; //in meters
	[Export] public float SpeedMultiplier = 1.5f;


	[ExportGroup("Managers")] [Export] public StateMachineManager StateManager;

	[ExportGroup("Camera Settings")] [Export]
	public float SwayAmplitude = 2.0f;

	[Export] public float SwaySpeed = 2.0f;
	[Export] public float SwimmingSpeed = 5.0f;
	public float Timer;


	public override void _Ready() {
		CameraAnchoring = GetNode<Marker3D>("CameraSmoothing");
		Timer = 0.0f;
		DefaultBsAmplitude = 0.0f;

		//hud elements
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_playerMainMenu = _mainMenu.Instantiate<MainMenu>();
		_hudLayer.AddChild(_playerMainMenu);
		_playerMainMenu.Visible = false;
		_playerMainMenu.ProcessMode = ProcessModeEnum.Always;
		_hudLayer.Layer = 2;
		_hudLayer.InteractLabel.Visible = false;
		_hudLayer.InteractLabel.Text = _interactText;
		_hudLayer.InteractLabel.Position = (GetViewport().GetVisibleRect().Size / 2) - new Vector2(-3, 3);

		//MousePosition = GetViewport().GetVisibleRect().Size / 2;
		//physics
		_rayCast.TargetPosition = Vector3.Forward * Reach;
		//_rayCast.Position = new Vector3(MousePosition.X,MousePosition.Y,0);

		//signals
		GameManager.Instance.MouseSenseChanged += SetSensitivity;
		GameManager.Instance.FovChanged += PlayerCamera.SetFov;

		_mouseSensitivity = SaveAndLoadManager.Instance.GetUserSetting().MouseSensitivity;
		PlayerCamera.SetFov(SaveAndLoadManager.Instance.GetUserSetting().Fov);
		AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["Walking"]);
	}

	private void SetSensitivity(float sensitivity) { _mouseSensitivity = sensitivity; }

	public override void _Process(double delta) {}

	public override void _PhysicsProcess(double delta) {
		MovementDirection = Input.GetVector("Left_Movement", "Right_Movement", "Backward_Movement", "Forward_Movement");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		MovementDirectionTranslation = MovementDirection.X * _cameraController.Basis.X - MovementDirection.Y * _cameraController.Basis.Z;

		GodotObject collider = _rayCast.GetCollider();
		if(collider is Interactable i) {
			CurrentInteractable = i;
			_hudLayer.InteractLabel.Visible = true;
		} else {
			CurrentInteractable = null;
			_hudLayer.InteractLabel.Visible = false;
		}
	}

	public override void _Input(InputEvent @event) {
		if(@event is InputEventMouseMotion eventMouseMotion) {
			_mousePosC = (eventMouseMotion.Relative / 1080 * Mathf.Pi) * _mouseSensitivity;
			_cameraController.RotateCamera(-_mousePosC, 4.0f, -4.0f);
		}

		if(Input.IsActionPressed("Interact_Action") && CurrentInteractable != null) {
			CurrentInteractable.Interact();
			GetViewport().SetInputAsHandled();
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


	public void AssignFootStepStreams(AudioStream[] streams) {
		AudioStreamRandomizer streamRandomizer = new AudioStreamRandomizer();

		for(int i = 0; i < streams.Length; i++) {
			streamRandomizer.AddStream(i, streams[i]);
		}

		_footstepPlayer.SetStream(streamRandomizer);
	}

	public void PlayFootstepSound() { _footstepPlayer.Play(); }

	public bool IsFootstepActive() { return _footstepPlayer.IsPlaying(); }
}