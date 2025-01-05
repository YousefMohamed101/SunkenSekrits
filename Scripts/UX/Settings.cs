using Godot;

public partial class Settings : Control
{
    [Signal]
    public delegate void SettingReturnEventHandler();
    
    private SettingData _userSettingData;
    private AudioStream _audioStream = GD.Load<AudioStream>("res://Assets/Audio/SoundFX/SliderandCheckin.wav");
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
    private Label[] _audioLabels;
    
    public override void _Ready()
    {
        
        //Audio tab Get Nodes
        _masterVolumeLabel =GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/MasterVolumeField/HBoxContainer2/MasterCounter");
        _voiceVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/VoiceVolumeField/HBoxContainer2/VoiceCounter");
        _bgmVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/BGMVolumeField/HBoxContainer2/Counter");
        _sfxVolumeLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/SoundEffectsField/HBoxContainer2/Counter");
        _audioLabels = [_masterVolumeLabel,_voiceVolumeLabel,_bgmVolumeLabel,_sfxVolumeLabel];
        _masterVolumeSlider = GetNode<HSlider>("%MasterVolumeSlider");
        _voiceVolumeSlider = GetNode<HSlider>("%VoiceVolumeSlider");
        _bgmVolumeSlider = GetNode<HSlider>("%BGMVolumeSlider");
        _sfxVolumeSlider = GetNode<HSlider>("%SFXVolumeSlider");
        
        //Audio tab Signal connect
        _masterVolumeSlider.ValueChanged += value => AudioSlidersChange(value,0);
        _voiceVolumeSlider.ValueChanged += value => AudioSlidersChange(value,1);
        _bgmVolumeSlider.ValueChanged += value => AudioSlidersChange(value,2);
        _sfxVolumeSlider.ValueChanged += value => AudioSlidersChange(value,3);
        
        
        //Game tab Get Nodes
        _mouseSensitivityLabel = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Game/MarginContainer/VBoxContainer/MouseSensetivityField/HBoxContainer/Counter");
        _mouseSensitivitySlider = GetNode<HSlider>("%MouseSesitivitySlider");
        _invertBtn = GetNode<CheckButton>("%InvertButtonCheck");
        _languageSelectorOptionButton = GetNode<OptionButton>("%LanguageSelector");
        
        //Game tab signal connect
        _mouseSensitivitySlider.ValueChanged += SetMouseSensitivitySlider;
        _invertBtn.Toggled += SetInvertMouse;
        _languageSelectorOptionButton.ItemSelected += SetLanguage;
        
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
        
        //Setting General function Get Nodes
        _uisfx = GetNode<AudioStreamPlayer>("%SettingUISFX");
        _btnBack = GetNode<Button>("%SettingsBack");
        _btnDefaultSetting = GetNode<Button>("%DefaultSettings");
        
        //Setting General function Signal Connection
        _btnBack.Pressed += ()=> EmitSignal("SettingReturn");
        _btnDefaultSetting.Pressed += SetDefaultSettingRuntime;
        
        //Load and initialize Settings
        LoadUserSettings();
        SetSettingRuntime(_userSettingData);




    }
    
    // Settings Menu states
    public void Open()
    {
        this.Visible = true;
    }
    public void Close()
    {
        this.Visible = false;
    }

    
    // System communication functions
    private void SetDefaultSettingRuntime()
    {
        SetSettingRuntime(SaveAndLoadManager.Instance.GetDefaultSetting());
        
    }
    
    // Function to set and initialize all setting and update the system
    private void SetSettingRuntime(SettingData setting)
    {
        AudioSlidersChange(setting.MasterVolume,0);
        AudioSlidersChange(setting.VoicesVolume,1);
        AudioSlidersChange(setting.BgmVolume,2);
        AudioSlidersChange(setting.SfxVolume,3);
        SetFrameLimitSlider(setting.FrameLimit);
        SetVSync(setting.VSync);
        SetWindowModeOptionButton(setting.WindowModeIndex);
        SetMouseSensitivitySlider(setting.MouseSensitivity);
        SetInvertMouse(setting.InvertedMode);
        SetLanguage(setting.Language);
    }
    
    private void LoadUserSettings()
    {
        _userSettingData = SaveAndLoadManager.Instance.GetUserSetting();
    }
    
    
    
    // Audio tab Functions
    // Set Audio buses volume and  Saves the data
    private void AudioSlidersChange(double change ,int index)
    {
        _uisfx.Stream = _audioStream;
        _uisfx.Play();
        AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb((float)change));
        _audioLabels[index].Text = Mathf.Floor(change*100).ToString();
        
        // Save Value
        switch (index)
        {
            case 0:
                _userSettingData.MasterVolume = (float)change;
                _masterVolumeSlider.Value = (float)change;
                _audioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
                break;
            case 1:
                _userSettingData.VoicesVolume = (float)change;
                _voiceVolumeSlider.Value = (float)change;
                _audioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
                break;
            case 2:
                _userSettingData.BgmVolume = (float)change;
                _bgmVolumeSlider.Value = (float)change;
                _audioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
                break;
            case 3:
                _userSettingData.SfxVolume = (float)change;
                _sfxVolumeSlider.Value = (float)change;
                _audioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
                break;
            default:
                break;
        }
        
    }
    
    
    
    
    
    
    // Graphic Tab Functions
    private void SetVSync(long Index)
    {
        _vsyncModeOptionButton.Selected = (int)Index;
        switch (Index)
        {
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
    private void SetFrameLimitSlider(double value)
    {
        _frameLimitSlider.Value = (int)value;
        if (value < 299)
        {
            Engine.MaxFps = (int)value;
            _frameLimitLabel.Text = Engine.MaxFps.ToString();
            
        }
        else
        {
            Engine.MaxFps = 99999999;
            _frameLimitLabel.Text = "Unlimited";
        }
        
        _userSettingData.FrameLimit = (int)value;
        SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
        
    }
    

    private void SetWindowModeOptionButton(long Index)
    {
        _windowModeOptionButton.Selected = (int)Index;
        switch (Index)
        {
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

    private void SetResolution(long value)
    {
        
    }
    
    // Game tab Functions
    private void SetMouseSensitivitySlider(double value)
    {
        _mouseSensitivitySlider.Value = (float)value;
        _mouseSensitivityLabel.Text = value.ToString();
        _userSettingData.MouseSensitivity = (float)value;
        SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
    }

    private void SetInvertMouse(bool value)
    {
        if (value)
        {
            _invertBtn.SetPressed(true);
            _userSettingData.InvertedMode = value;
            SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
            return;
        }
        _invertBtn.SetPressed(false);
        _userSettingData.InvertedMode = value;
        SaveAndLoadManager.Instance.SaveUserSetting(_userSettingData);
    }

    private void SetLanguage(long code)
    {
        _languageSelectorOptionButton.Selected = (int)code;
        switch (code)
        {
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
            default:
                break;
        }
    }
}
