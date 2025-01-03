using Godot;
using System;

public partial class Settings : Control
{
    [Signal]
    public delegate void SettingReturnEventHandler();
    
    private SettingData UserSettingData;
    private AudioStream asStream = GD.Load<AudioStream>("res://Assets/Audio/SoundFX/SliderandCheckin.wav");
    private Button btnBack;
    private Button btnDefaultSetting;
    private CheckButton InvertBtn;
    private AudioStreamPlayer aspUISFX;
    private HSlider hsMasterVolume;
    private HSlider hsVoiceVolume;
    private HSlider hsBGMVolume;
    private HSlider hsSFXVolume;
    private HSlider hsFrameLimit;
    private HSlider hsMouseSensitivity;
    private bool bIsAudioPlaying = false;
    private OptionButton obWindowMode;
    private OptionButton obResolution;
    private OptionButton obVSYNCMode;
    private OptionButton obLanguageSelector;
    private Label lblMasterVolume;
    private Label lblVoiceVolume;
    private Label lblBGMVolume;
    private Label lblSFXVolume;
    private Label lblFrameLimit;
    private Label lblMouseSensitivity;
    private Label[] lblAudioLabels;
    
    public override void _Ready()
    {
        
        //Audio tab Get Nodes
        lblMasterVolume =GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/MasterVolumeField/HBoxContainer2/MasterCounter");
        lblVoiceVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/VoiceVolumeField/HBoxContainer2/VoiceCounter");
        lblBGMVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/BGMVolumeField/HBoxContainer2/Counter");
        lblSFXVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/SoundEffectsField/HBoxContainer2/Counter");
        lblAudioLabels = [lblMasterVolume,lblVoiceVolume,lblBGMVolume,lblSFXVolume];
        hsMasterVolume = GetNode<HSlider>("%MasterVolumeSlider");
        hsVoiceVolume = GetNode<HSlider>("%VoiceVolumeSlider");
        hsBGMVolume = GetNode<HSlider>("%BGMVolumeSlider");
        hsSFXVolume = GetNode<HSlider>("%SFXVolumeSlider");
        
        //Audio tab Signal connect
        hsMasterVolume.ValueChanged += Value => AudioSlidersChange(Value,0);
        hsVoiceVolume.ValueChanged += Value => AudioSlidersChange(Value,1);
        hsBGMVolume.ValueChanged += Value => AudioSlidersChange(Value,2);
        hsSFXVolume.ValueChanged += Value => AudioSlidersChange(Value,3);
        
        
        //Game tab Get Nodes
        lblMouseSensitivity = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Game/MarginContainer/VBoxContainer/MouseSensetivityField/HBoxContainer/Counter");
        hsMouseSensitivity = GetNode<HSlider>("%MouseSesitivitySlider");
        InvertBtn = GetNode<CheckButton>("%InvertButtonCheck");
        obLanguageSelector = GetNode<OptionButton>("%LanguageSelector");
        
        //Game tab signal connect
        hsMouseSensitivity.ValueChanged += SetMouseSensitivity;
        InvertBtn.Toggled += SetInvertMouse;
        obLanguageSelector.ItemSelected += SetLanguage;
        
        //Graphic tab Get Nodes
        obVSYNCMode = GetNode<OptionButton>("%VSyncSelector");
        obWindowMode = GetNode<OptionButton>("%WindowModeSelector");
        obResolution = GetNode<OptionButton>("%ResolutionSelector");
        hsFrameLimit = GetNode<HSlider>("%FrameLimitSlider");
        lblFrameLimit = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Graphic/MarginContainer/VBoxContainer3/HBoxContainer4/HBoxContainer/Counter");
        
       //Graphic tab Signal connection
        hsFrameLimit.ValueChanged += SetFrameLimit;
        obVSYNCMode.ItemSelected += SetVSync;
        obWindowMode.ItemSelected += SetWindowMode;
        
        //Setting General function Get Nodes
        aspUISFX = GetNode<AudioStreamPlayer>("%SettingUISFX");
        btnBack = GetNode<Button>("%SettingsBack");
        btnDefaultSetting = GetNode<Button>("%DefaultSettings");
        
        //Setting General function Signal Connection
        btnBack.Pressed += ()=> EmitSignal("SettingReturn");
        btnDefaultSetting.Pressed += ()=> SetDefaultSettingRuntime();
        
        //Load and initialize Settings
        LoadUserSettings();
        SetSettingRuntime(UserSettingData);




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
    private void SetSettingRuntime(SettingData InputSettingData)
    {
        AudioSlidersChange(InputSettingData.fMasterV,0);
        AudioSlidersChange(InputSettingData.fVoicesV,1);
        AudioSlidersChange(InputSettingData.fBGMV,2);
        AudioSlidersChange(InputSettingData.fSFXV,3);
        SetFrameLimit(InputSettingData.fFrameLimit);
        SetVSync(InputSettingData.iVSync);
        SetWindowMode(InputSettingData.iWindowModeIndex);
        SetMouseSensitivity(InputSettingData.fMouseSensitivity);
        SetInvertMouse(InputSettingData.bInvertedControls);
        SetLanguage(InputSettingData.iLanguage);
    }
    
    //Load Setting Data from setting resource
    private void LoadUserSettings()
    {
        UserSettingData = SaveAndLoadManager.Instance.GetUserSetting();
    }
    
    
    
    // Audio tab Functions
    // Set Audio buses volume and  Saves the data
    private void AudioSlidersChange(double change ,int index)
    {
        aspUISFX.Stream = asStream;
        aspUISFX.Play();
        AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb((float)change));
        lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();
        
        // Save Value
        switch (index)
        {
            case 0:
                UserSettingData.fMasterV = (float)change;
                hsMasterVolume.Value = (float)change;
                lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 1:
                UserSettingData.fVoicesV = (float)change;
                hsVoiceVolume.Value = (float)change;
                lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 2:
                UserSettingData.fBGMV = (float)change;
                hsBGMVolume.Value = (float)change;
                lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 3:
                UserSettingData.fSFXV = (float)change;
                hsSFXVolume.Value = (float)change;
                lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            default:
                break;
        }
        
    }
    
    
    
    
    
    
    // Graphic Tab Functions
    private void SetVSync(long Index)
    {
        obVSYNCMode.Selected = (int)Index;
        switch (Index)
        {
            case 0:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                UserSettingData.iVSync = 0;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 1:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
                UserSettingData.iVSync = 1;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 2:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
                UserSettingData.iVSync = 2;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            default:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                break;
        }   

    }
    
    // control the frame Limit of Game and save data and initialize
    private void SetFrameLimit(double value)
    {
        hsFrameLimit.Value = (int)value;
        if (value < 299)
        {
            Engine.MaxFps = (int)value;
            lblFrameLimit.Text = Engine.MaxFps.ToString();
            
        }
        else
        {
            Engine.MaxFps = 99999999;
            lblFrameLimit.Text = "Unlimited";
        }
        
        UserSettingData.fFrameLimit = (int)value;
        SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
        
    }
    

    private void SetWindowMode(long Index)
    {
        obWindowMode.Selected = (int)Index;
        switch (Index)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                UserSettingData.iWindowModeIndex = 0;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                UserSettingData.iWindowModeIndex = 1;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
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
    private void SetMouseSensitivity(double value)
    {
        hsMouseSensitivity.Value = (float)value;
        lblMouseSensitivity.Text = value.ToString();
        UserSettingData.fMouseSensitivity = (float)value;
        SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
    }

    private void SetInvertMouse(bool value)
    {
        if (value)
        {
            InvertBtn.SetPressed(true);
            UserSettingData.bInvertedControls = value;
            SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
            return;
        }
        InvertBtn.SetPressed(false);
        UserSettingData.bInvertedControls = value;
        SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
    }

    private void SetLanguage(long code)
    {
        obLanguageSelector.Selected = (int)code;
        switch (code)
        {
            case 0:
                //TranslationServer.SetLocale("en-US");
                UserSettingData.iLanguage = 0;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 1:
                UserSettingData.iLanguage = 1;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 2:
                GD.Print("Selected");
                UserSettingData.iLanguage = 2;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            case 3:
                UserSettingData.iLanguage = 3;
                SaveAndLoadManager.Instance.SaveUserSetting(UserSettingData);
                break;
            default:
                break;
        }
    }
}
