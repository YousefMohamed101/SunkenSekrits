using Godot;
using System;

public partial class Settings : Control
{
    [Signal]
    public delegate void SettingReturnEventHandler();
    
    private AudioStream asStream = GD.Load<AudioStream>("res://Assets/Audio/SoundFX/SliderandCheckin.wav");
    private Button btnBack;
    private AudioStreamPlayer aspUISFX;
    private HSlider hsMasterVolume;
    private HSlider hsVoiceVolume;
    private HSlider hsBGMVolume;
    private HSlider hsSFXVolume;
    private HSlider hsFrameLimit;
    private HSlider hsMouseSensitivity;
    private bool bIsAudioPlaying = false;
    private SettingData sdSettingData;
    private OptionButton obWindowMode;
    private OptionButton obResolution;
    private OptionButton obVSYNCMode;
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
        
        //Game tab signal connect
        hsMouseSensitivity.ValueChanged += SetMouseSensitivity;
        
        
        //Graphic tab Get Nodes
        obVSYNCMode = GetNode<OptionButton>("%VSyncSelector");
        obWindowMode = GetNode<OptionButton>("%WindowModeSelector");
        obResolution = GetNode<OptionButton>("%ResolutionSelector");
        hsFrameLimit = GetNode<HSlider>("%FrameLimitSlider");
        lblFrameLimit = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Graphic/MarginContainer/GridContainer/HBoxContainer/Counter");
        
       //Graphic tab Signal connection
        hsFrameLimit.ValueChanged += SetFrameLimit;
        obVSYNCMode.ItemSelected += SetVSync;
        obWindowMode.ItemSelected += SetWindowMode;
        
        //Setting General function Get Nodes
        aspUISFX = GetNode<AudioStreamPlayer>("%SettingUISFX");
        btnBack = GetNode<Button>("%SettingsBack");
        
        //Setting General function Signal Connection
        btnBack.Pressed += ()=> EmitSignal("SettingReturn");
        
        
        
        
    }
    
    public void Open()
    {
        this.Visible = true;
    }
    public void Close()
    {
        this.Visible = false;
    }

    private void AudioSlidersChange(double change ,int index)
    {
        aspUISFX.Stream = asStream;
        aspUISFX.Play();
        AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb((float)change));
        lblAudioLabels[index].Text = Mathf.Floor(change*100).ToString();    
        
    }

    private void SetFrameLimit(double value)
    {
        Engine.MaxFps = (int)value;
        lblFrameLimit.Text = Engine.MaxFps.ToString();
    }
    private void SetVSync(long Index)
    {

        switch (Index)
        {
            case 0:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                break;
            case 1:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
                break;
            case 2:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
                break;
            default:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                break;
        }
        
        
        

    }

    private void SetMouseSensitivity(double value)
    {
        lblMouseSensitivity.Text = (Mathf.Floor(value*100f)/100f).ToString();
    }

    private void SetWindowMode(long Index)
    {
        switch (Index)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            default:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
        }
    }

    private void SetResolution(long value)
    {
        
    }
}
