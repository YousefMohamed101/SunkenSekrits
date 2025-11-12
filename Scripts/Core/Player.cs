using System;
using System.Text.RegularExpressions;
using Godot;

public partial class Player : CharacterBody3D {
	[Export] public CameraController _cameraController;

	[ExportGroup("Nodes")] [Export] private AudioStreamPlayer3D _footstepPlayer;
	[Export] private PackedScene _mainMenu;
	private Vector2 _mousePosC;
	private float _mouseSensitivity = 1.0f;
	private MainMenu _playerMainMenu;
	private AudioStream[] _sounds;

	public Car ActiveCar;
	[Export] public float BobbingAmplitude = 0.25f;
	[Export] public float BobbingSpeed = 2.0f;
	public Vector3 CalcVelocity;
	[Export] public Marker3D CameraAnchoring;
	public Interactable CurrentInteractable;
	public float DefaultBsAmplitude;
	[Export] public DialogueHud DialogueHud;
	[Export] public float FallSpeed = 5.0f;

	[Export] private Marker3D GearSlot;
	[Export] private Marker3D GLowHold;
	[Export] public RayCast3D GroundRayCast;
	[Export] public Hud HudLayer;
	public string InteractText = "Press Interact";
	[Export] public float JumpForce = 5.0f;
	public Vector2 MousePosition;

	//runtime 
	public Vector2 MovementDirection;
	public Vector3 MovementDirectionTranslation;

	[ExportGroup("Player Stats")] [Export] public float MovementSpeed = 100f;

	[Export] public NarcosisEffect NarcosisEffect;
	[Export] public Camera3D PlayerCamera;
	[Export] public RayCast3D RayCast;
	[Export] public int Reach = 2; //in meters
	[Export] public float SpeedMultiplier = 1.5f;


	[ExportGroup("Managers")] [Export] public StateMachineManager StateManager;


	private GlowStick stick;

	[ExportGroup("Camera Settings")] [Export]
	public float SwayAmplitude = 2.0f;

	[Export] public float SwaySpeed = 2.0f;
	[Export] public float SwimmingSpeed = 5.0f;
	[Export] private Vector3 ThrowDirection;
	[Export] private float ThrowForce;
	public float Timer;


	public void EquipGlowstick(GlowStick sticks) {
		if(stick != null) {
			return;
		}

		GD.Print("EquipGlowstick");
		stick = sticks;
		stick.GlobalPosition = GLowHold.GlobalPosition;
		stick.GlobalRotation = GLowHold.GlobalRotation;
		stick.Reparent(GLowHold);
	}

	public void EquipGear(FullGear gear) {
		gear.GlobalPosition = GearSlot.GlobalPosition;
		gear.GlobalRotation = GearSlot.GlobalRotation;


		gear.Reparent(this);
	}


	public override void _Ready() {
		Timer = 0.0f;
		DefaultBsAmplitude = 0.0f;
		stick = null;

		//hud elements
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_playerMainMenu = _mainMenu.Instantiate<MainMenu>();
		HudLayer.AddChild(_playerMainMenu);
		_playerMainMenu.Visible = false;
		_playerMainMenu.ProcessMode = ProcessModeEnum.Always;
		HudLayer.Layer = 2;
		HudLayer.InteractLabel.Visible = false;
		HudLayer.InteractLabel.Text = InteractText;
		HudLayer.InteractLabel.Position = (GetViewport().GetVisibleRect().Size / 2) - new Vector2(-3, 3);

		//MousePosition = GetViewport().GetVisibleRect().Size / 2;
		//physics
		RayCast.TargetPosition = Vector3.Forward * Reach;
		//_rayCast.Position = new Vector3(MousePosition.X,MousePosition.Y,0);

		//signals
		GameManager.Instance.MouseSenseChanged += SetSensitivity;
		GameManager.Instance.FovChanged += PlayerCamera.SetFov;
		GameManager.Instance.RideCar += OnCarInteract;
		GameManager.Instance.EquipGlowstick += EquipGlowstick;
		GameManager.Instance.EquipGear += EquipGear;
		_mouseSensitivity = SaveAndLoadManager.Instance.GetUserSetting().MouseSensitivity;
		PlayerCamera.SetFov(SaveAndLoadManager.Instance.GetUserSetting().Fov);
		AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["Walking"]);
	}

	private void SetSensitivity(float sensitivity) { _mouseSensitivity = sensitivity; }

	public override void _Process(double delta) {}

	public override void _PhysicsProcess(double delta) {
		MovementDirection = Input.GetVector("Left_Movement", "Right_Movement", "Backward_Movement", "Forward_Movement");
		//MovementDirectionTranslation = (Transform.Basis * new Vector3(MovementDirection.X, 0, -MovementDirection.Y)).Normalized();
		MovementDirectionTranslation = MovementDirection.X * Basis.X - MovementDirection.Y * Basis.Z;
	}

	public override void _Input(InputEvent @event) {
		if(@event is InputEventMouseMotion eventMouseMotion) {
			_mousePosC = (eventMouseMotion.Relative / 1080 * Mathf.Pi) * _mouseSensitivity;
			_cameraController.RotateCamera(-_mousePosC, 1.0f, -1.0f);
		}

		if(Input.IsActionPressed("Use_Action")) {
			if(stick != null) {
				stick.ThrowItem((-CameraAnchoring.GlobalBasis.Z + ThrowDirection).Normalized() * ThrowForce);
				stick = null;
			}
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

	private void OnCarInteract(Car car) {
		ActiveCar = car;
		StateManager.TransitionToState("Driving");
	}


	public void AssignFootStepStreams(AudioStream[] streams) {
		AudioStreamRandomizer streamRandomizer = new AudioStreamRandomizer();

		for(int i = 0; i < streams.Length; i++) {
			streamRandomizer.AddStream(i, streams[i]);
		}

		_footstepPlayer.SetStream(streamRandomizer);
	}

	public void PlayFootstepSound() { _footstepPlayer.Play(); }
	public void StopFootstepSound() { _footstepPlayer.Stop(); }
	public bool IsFootstepActive() { return _footstepPlayer.IsPlaying(); }
}