using System.Linq;
using Godot;
using Godot.Collections;

public partial class Settings : Control {
	[Signal]
	public delegate void SettingReturnEventHandler();
	
	[Export] private PackedScene _keybindfield;
	private InputEvent _previousEvent;
	private StringName _actionNa;
	private Array<StringName> _actionList;
	private KeyBind  _keybindFieldCasted;
	private bool _isremaping;
	private string _actionToSet;
	private VBoxContainer _movementBinds;
	private VBoxContainer _actionBinds;
	private SettingData _userSettingData;
	private AudioStream _audioStream = GD.Load<AudioStream>("res://Assets/Audio/SoundFX/SliderandCheckin.wav");
	private Button _buttonKeybindSet;
	private Button _btnBack;
	private Button _btnDefaultSetting;
	private CheckButton _invertBtn;
	private AudioStreamPlayer _uisfx;
	private HSlider _masterVolumeSlider;
	private HSlider _voiceVolumeSlider;
	private HSlider _bgmVolumeSlider;
	private HSlider _sfxVolumeSlider;
	private HSlider _frameLimitSlider;
	private HSlider _mouseSensitivitySlider;
	private HSlider _FOVSlider;
	private OptionButton _windowModeOptionButton;
	private OptionButton _resolutionOptionButton;
	private OptionButton _vsyncModeOptionButton;
	private OptionButton _languageSelectorOptionButton;
	private Label _masterVolumeLabel;
	private Label _voiceVolumeLabel;
	private Label _bgmVolumeLabel;
	private Label _sfxVolumeLabel;
	private Label _frameLimitLabel;
	private Label _mouseSensitivityLabel;
	private Label _FOVLabel;
	private Label[] _audioLabels;
	

	public override void _Ready() {
		
		//Audio tab Get Nodes
		_masterVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/MasterVolumeField/HBoxContainer2/MasterCounter");
		_voiceVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/VoiceVolumeField/HBoxContainer2/VoiceCounter");
		_bgmVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/BGMVolumeField/HBoxContainer2/Counter");
		_sfxVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/SoundEffectsField/HBoxContainer2/Counter");
		_audioLabels = [_masterVolumeLabel, _voiceVolumeLabel, _bgmVolumeLabel, _sfxVolumeLabel];
		_masterVolumeSlider = GetNode<HSlider>("%MasterVolumeSlider");
		_voiceVolumeSlider = GetNode<HSlider>("%VoiceVolumeSlider");
		_bgmVolumeSlider = GetNode<HSlider>("%BGMVolumeSlider");
		_sfxVolumeSlider = GetNode<HSlider>("%SFXVolumeSlider");

		//Audio tab Signal connect
		_masterVolumeSlider.ValueChanged += value => AudioSlidersChange(value, 0);
		_voiceVolumeSlider.ValueChanged += value => AudioSlidersChange(value, 1);
		_bgmVolumeSlider.ValueChanged += value => AudioSlidersChange(value, 2);
		_sfxVolumeSlider.ValueChanged += value => AudioSlidersChange(value, 3);


		//Game tab Get Nodes
		_mouseSensitivityLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Game/MarginContainer/VBoxContainer/MouseSensetivityField/HBoxContainer/Counter");
		_mouseSensitivitySlider = GetNode<HSlider>("%MouseSesitivitySlider");
		_invertBtn = GetNode<CheckButton>("%InvertButtonCheck");
		_languageSelectorOptionButton = GetNode<OptionButton>("%LanguageSelector");
		_FOVSlider = GetNode<HSlider>("%FOVSlider");
		_FOVLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Game/MarginContainer/VBoxContainer/FOVField/FOVLabel");

		//Game tab signal connect
		_mouseSensitivitySlider.ValueChanged += SetMouseSensitivitySlider;
		_invertBtn.Toggled += SetInvertMouse;
		_languageSelectorOptionButton.ItemSelected += SetLanguage;
		_FOVSlider.ValueChanged += SetFOV;
		
		

		//Graphic tab Get Nodes
		_vsyncModeOptionButton = GetNode<OptionButton>("%VSyncSelector");
		_windowModeOptionButton = GetNode<OptionButton>("%WindowModeSelector");
		_resolutionOptionButton = GetNode<OptionButton>("%ResolutionSelector");
		_frameLimitSlider = GetNode<HSlider>("%FrameLimitSlider");
		_frameLimitLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Graphic/MarginContainer/VBoxContainer3/HBoxContainer4/HBoxContainer/Counter");

		//Graphic tab Signal connection
		_frameLimitSlider.ValueChanged += SetFrameLimitSlider;
		_vsyncModeOptionButton.ItemSelected += SetVSync;
		_windowModeOptionButton.ItemSelected += SetWindowModeOptionButton;
		
		
		// Keybind Tab
		_movementBinds = GetNode<VBoxContainer>("%MovementBinds");
		_actionBinds = GetNode<VBoxContainer>("%ActionBinds");
		_actionList = InputMap.GetActions();
		
		KeyBindTabInit();
		
		
		//Setting General function Get Nodes
		_uisfx = GetNode<AudioStreamPlayer>("%SettingUISFX");
		_btnBack = GetNode<Button>("%SettingsBack");
		_btnDefaultSetting = GetNode<Button>("%DefaultSettings");

		//Setting General function Signal Connection
		_btnBack.Pressed += () => EmitSignal("SettingReturn");
		_btnDefaultSetting.Pressed += SetDefaultSettingRuntime;

		//Load and initialize Settings
		LoadUserSettings();
		SetSettingRuntime(_userSettingData);
		GameManager.Instance.EmitSignal("FOVChanged", (float)_FOVSlider.Value);
		GameManager.Instance.EmitSignal("MouseSenseChanged", (float)_mouseSensitivitySlider.Value);
	}


	public override void _Input(InputEvent @event) {
		if(_isremaping) {
			
			if(@event is InputEventKey eventKey && @event.IsPressed()) {
				if(!IsKeyAvailable(@event)) {
					SetKeybind(_actionNa,null);
				}
				SetKeybind(_actionToSet, @event);
				_buttonKeybindSet.Text = InputMap.ActionGetEvents(_actionToSet)[0].AsText().TrimSuffix(" (Physical)");
				_isremaping = false;
				_buttonKeybindSet = null;
				_actionToSet = null;
				AcceptEvent();
			}else if(@event is InputEventMouseButton aevent && @event.IsPressed()) {
				if(aevent.DoubleClick) {
					aevent.DoubleClick = false;
				}
				if(!IsKeyAvailable(@event)) {
					SetKeybind(_actionNa,null);
				}
			

				SetKeybind(_actionToSet, @event);
				_buttonKeybindSet.Text = InputMap.ActionGetEvents(_actionToSet)[0].AsText().TrimSuffix(" (Physical)");
				_buttonKeybindSet.Text = _buttonKeybindSet.Text.TrimSuffix(" Button");
				_isremaping = false;
				_buttonKeybindSet = null;
				_actionToSet = null;
				AcceptEvent();
			}
		}
	}

	// Settings Menu states
	public void Open() { Visible = true; }
	public void Close() { Visible = false; }


	// System communication functions
	private void SetDefaultSettingRuntime() { SetSettingRuntime(SaveAndLoadManager.Instance.GetDefaultSetting()); }

	// Function to set and initialize all setting and update the system
	private void SetSettingRuntime(SettingData setting) {
		AudioSlidersChange(setting.MasterVolume, 0);
		AudioSlidersChange(setting.VoicesVolume, 1);
		AudioSlidersChange(setting.BgmVolume, 2);
		AudioSlidersChange(setting.SfxVolume, 3);
		SetFrameLimitSlider(setting.FrameLimit);
		SetVSync(setting.VSync);
		SetWindowModeOptionButton(setting.WindowModeIndex);
		SetMouseSensitivitySlider(setting.MouseSensitivity);
		SetInvertMouse(setting.InvertedMode);
		SetLanguage(setting.Language);
		SetFOV(setting.Fov);
		SetKeybind("Forward (Movement)", setting.ForwardButton);
		SetKeybind("Backward (Movement)", setting.BackwardButton);
		SetKeybind("Right (Movement)", setting.RightButton);
		SetKeybind("Left (Movement)", setting.LeftButton);
		SetKeybind("Jump (Movement)", setting.JumpButton);
		SetKeybind("Run (Movement)",setting.RunButton);
		SetKeybind("Crouch (Movement)", setting.CrouchButton);
		SetKeybind("Interact (Action)", setting.InteractButton);
		SetKeybind("Use (Action)", setting.UseButton);
		SetKeybind("Aim (Action)", setting.AimButton);
		
	}

	private void LoadUserSettings() { _userSettingData = SaveAndLoadManager.Instance.GetUserSetting(); }


	// Audio tab Functions
	// Set Audio buses volume and  Saves the data
	private void AudioSlidersChange(double change, int index) {
		_uisfx.Stream = _audioStream;
		_uisfx.Play();
		AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb((float)change));
		_audioLabels[index].Text = Mathf.Floor(change * 100).ToString();

		// Save Value
		switch(index) {
			case 0:
				_userSettingData.MasterVolume = (float)change;
				_masterVolumeSlider.Value = (float)change;
				_audioLabels[index].Text = Mathf.Floor(change * 100).ToString();
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 1:
				_userSettingData.VoicesVolume = (float)change;
				_voiceVolumeSlider.Value = (float)change;
				_audioLabels[index].Text = Mathf.Floor(change * 100).ToString();
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 2:
				_userSettingData.BgmVolume = (float)change;
				_bgmVolumeSlider.Value = (float)change;
				_audioLabels[index].Text = Mathf.Floor(change * 100).ToString();
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 3:
				_userSettingData.SfxVolume = (float)change;
				_sfxVolumeSlider.Value = (float)change;
				_audioLabels[index].Text = Mathf.Floor(change * 100).ToString();
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			default:
				break;
		}
	}


	// Graphic Tab Functions
	private void SetVSync(long index) {
		_vsyncModeOptionButton.Selected = (int)index;
		switch(index) {
			case 0:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
				_userSettingData.VSync = 0;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 1:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
				_userSettingData.VSync = 1;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 2:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
				_userSettingData.VSync = 2;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			default:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
				break;
		}
	}

	// control the frame Limit of Game and save data and initialize
	private void SetFrameLimitSlider(double value) {
		_frameLimitSlider.Value = (int)value;
		if(value < 299) {
			Engine.MaxFps = (int)value;
			_frameLimitLabel.Text = Engine.MaxFps.ToString();
		} else {
			Engine.MaxFps = 99999999;
			_frameLimitLabel.Text = "Unlimited";
		}

		_userSettingData.FrameLimit = (int)value;
		SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
	}


	private void SetWindowModeOptionButton(long index) {
		_windowModeOptionButton.Selected = (int)index;
		switch(index) {
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				_userSettingData.WindowModeIndex = 0;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				_userSettingData.WindowModeIndex = 1;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			default:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
		}
	}

	private void SetResolution(long value) {}

	
	
	// Game tab Functions
	private void SetMouseSensitivitySlider(double value) {
		_mouseSensitivitySlider.Value = (float)value;
		GameManager.Instance.EmitSignal("MouseSenseChanged", (float)value);
		_mouseSensitivityLabel.Text = (Mathf.Floor(value * 100)/100).ToString();
		_userSettingData.MouseSensitivity = (float)value;
		SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
	}
	
	private void SetFOV(double value) {
		_FOVSlider.Value = (float)value;
		GameManager.Instance.EmitSignal("FOVChanged", (float)value);
		_FOVLabel.Text = (Mathf.Floor(value * 100)/100).ToString();
		_userSettingData.Fov = (float)value;
		SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
	}


	private void SetInvertMouse(bool value) {
		if(value) {
			_invertBtn.SetPressed(true);
			_userSettingData.InvertedMode = value;
			SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
			return;
		}

		_invertBtn.SetPressed(false);
		_userSettingData.InvertedMode = value;
		SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
	}

	private void SetLanguage(long code) {
		_languageSelectorOptionButton.Selected = (int)code;
		switch(code) {
			case 0:
				//TranslationServer.SetLocale("en-US");
				_userSettingData.Language = 0;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 1:
				_userSettingData.Language = 1;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 2:
				GD.Print("Selected");
				_userSettingData.Language = 2;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			case 3:
				_userSettingData.Language = 3;
				SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
				break;
			
		}
	}

	private void KeyBindTabInit() {
		
		foreach(string action in _actionList) {
			
			_keybindFieldCasted = _keybindfield.Instantiate<KeyBind>();
			Button keybindButton;
			if(action.Contains(" (Movement)")) {
				_movementBinds.AddChild(_keybindFieldCasted);
				keybindButton = _keybindFieldCasted.KeyBindButton;
				_keybindFieldCasted.KeyBindLabel.Text = action.TrimSuffix(" (Movement)");
				if(InputMap.ActionGetEvents(action).Count > 0) {
					_keybindFieldCasted.KeyBindButton.Text = InputMap.ActionGetEvents(action)[0].AsText().TrimSuffix(" (Physical)");
				} else {
					_keybindFieldCasted.KeyBindButton.Text = "Empty";
				}

				keybindButton.Pressed += ()=>SetKeybindButton(keybindButton, action);
				
			} else if(action.Contains(" (Action)")) {
				_actionBinds.AddChild(_keybindFieldCasted);
				keybindButton = _keybindFieldCasted.KeyBindButton;
				_keybindFieldCasted.KeyBindLabel.Text = action.TrimSuffix(" (Action)");
				if(InputMap.ActionGetEvents(action).Count > 0) {
					_keybindFieldCasted.KeyBindButton.Text = InputMap.ActionGetEvents(action)[0].AsText().TrimSuffix(" (Physical)");
				} else {
					_keybindFieldCasted.KeyBindButton.Text = "Empty";
				}

				keybindButton.Pressed += ()=>SetKeybindButton(keybindButton, action);
				if(_keybindFieldCasted.KeyBindButton.Text.Contains(" Mouse")) {
					_keybindFieldCasted.KeyBindButton.Text = _keybindFieldCasted.KeyBindButton.Text.TrimSuffix(" Button");
				}
			}
			
		}
	}

	private void SetKeybindButton(Button btn, string action) {
		if(_isremaping == false) {
			_isremaping = true;
			_actionToSet = action;
			if(InputMap.ActionGetEvents(action).Count > 0) {
				_previousEvent = InputMap.ActionGetEvents(action)[0];
				InputMap.ActionEraseEvents(action);
			}

			_buttonKeybindSet = btn;
			btn.Text = "Assign a Key";
			
		} 
	}

	private void SetKeybind(StringName action, InputEvent @event) {
		
		if(InputMap.ActionGetEvents(action).Count() > 0) {
			InputMap.ActionEraseEvents(action);
		}

		if(@event != null) {
			InputMap.ActionAddEvent(action, @event);
		}
		
		Button keybindass = FindKeybindButton(action);
		if(keybindass != null && @event != null) {
			keybindass.Text = InputMap.ActionGetEvents(action)[0].AsText().TrimSuffix(" (Physical)");
			if (keybindass.Text.Contains(" Mouse")) {
				keybindass.Text = keybindass.Text.TrimSuffix(" Button");
			}
		} else {
			keybindass.Text = "Empty";
		}
		
			// Save the updated settings
			switch(action) {
				case "Forward (Movement)":
					_userSettingData.ForwardButton = @event;
					break;
				case "Backward (Movement)":
					_userSettingData.BackwardButton = @event;
					break;
				case "Right (Movement)":
					_userSettingData.RightButton = @event;
					break;
				case "Left (Movement)":
					_userSettingData.LeftButton = @event;
					break;
				case "Jump (Movement)":
					_userSettingData.JumpButton = @event;
					break;
				case "Run (Movement)":
					_userSettingData.RunButton = @event;
					break;
				case "Crouch (Movement)":
					_userSettingData.CrouchButton = @event;
					break;
				case "Interact (Action)":
					_userSettingData.InteractButton = @event;
					break;
				case "Use (Action)":
					_userSettingData.UseButton = @event;
					break;
				case "Aim (Action)":
					_userSettingData.AimButton = @event;
					break;
			}
			SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
	}
	
	private Button FindKeybindButton(StringName action) {
		// Check movement binds container
		foreach (var child in _movementBinds.GetChildren()) {
			if (child is KeyBind keybind) {
				string actionName = action.ToString();
				string labelText = keybind.KeyBindLabel.Text;
                
				if (actionName.StartsWith(labelText)) {
					return keybind.KeyBindButton;
				}
			}
		}
        
		// Check action binds container
		foreach (var child in _actionBinds.GetChildren()) {
			if (child is KeyBind keybind) {
				string actionName = action.ToString();
				string labelText = keybind.KeyBindLabel.Text;
                
				if (actionName.StartsWith(labelText)) {
					return keybind.KeyBindButton;
				}
			}
		}
        
		return null;
	}


	
	private bool IsKeyAvailable(InputEvent @event) {
		foreach(string actionNa in _actionList) {
			if(actionNa.EndsWith("(Movement)") || actionNa.EndsWith("(Action)")) {
				if(InputMap.ActionGetEvents(actionNa).Count > 0) {
					var events = InputMap.ActionGetEvents(actionNa)[0];
					_actionNa = actionNa;
					if(@event is InputEventKey eventKey && events is InputEventKey existingEvent) {
						if(eventKey.Keycode == existingEvent.Keycode) {

							return false;
						}
					} else if(@event is InputEventMouseButton eventMouse && events is InputEventMouseButton existingEventMouse) {
						if(eventMouse.ButtonIndex == existingEventMouse.ButtonIndex) {
							
							return false;
						}
					}
				}
			}
		}
		return true;
	}
	
}