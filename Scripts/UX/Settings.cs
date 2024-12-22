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
    private bool bIsAudioPlaying = false;
    private SettingData sdSettingData;
    private OptionButton obWindowMode;
    private OptionButton obResolution;
    private Label lblMasterVolume;
    private Label lblVoiceVolume;
    private Label lblBGMVolume;
    private Label lblSFXVolume;
    private Label[] lblAudioLabels;
    public override void _Ready()
    {
        lblMasterVolume =GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/MasterVolumeField/HBoxContainer2/MasterCounter");
        lblVoiceVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/VoiceVolumeField/HBoxContainer2/VoiceCounter");
        lblBGMVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/BGMVolumeField/HBoxContainer2/Counter");
        lblSFXVolume = GetNode<Label>("CenterContainer/VBoxContainer/TabContainer/Audio/MarginContainer/AudioContolBoard/SoundEffectsField/HBoxContainer2/Counter");
        lblAudioLabels = [lblMasterVolume,lblVoiceVolume,lblBGMVolume,lblSFXVolume];
        hsMasterVolume = GetNode<HSlider>("%MasterVolumeSlider");
        hsVoiceVolume = GetNode<HSlider>("%VoiceVolumeSlider");
        hsBGMVolume = GetNode<HSlider>("%BGMVolumeSlider");
        hsSFXVolume = GetNode<HSlider>("%SFXVolumeSlider");
        aspUISFX = GetNode<AudioStreamPlayer>("%SettingUISFX");
        btnBack = GetNode<Button>("%SettingsBack");
        obWindowMode = GetNode<OptionButton>("%WindowModeSelector");
        obResolution = GetNode<OptionButton>("%ResolutionSelector");
        hsFrameLimit = GetNode<HSlider>("%FrameLimitSlider");
        
        btnBack.Pressed += ()=> EmitSignal("SettingReturn");
        hsMasterVolume.ValueChanged += Value => AudioSlidersChange(Value,0);
        hsVoiceVolume.ValueChanged += Value => AudioSlidersChange(Value,1);
        hsBGMVolume.ValueChanged += Value => AudioSlidersChange(Value,2);
        hsSFXVolume.ValueChanged += Value => AudioSlidersChange(Value,3);
        hsFrameLimit.ValueChanged += SetFrameLimit;
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
    }
    private void SeVSync(int Index)
    {
        
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
        
    }
}
