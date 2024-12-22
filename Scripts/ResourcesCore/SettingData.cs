using Godot;
using System;

[GlobalClass]
public partial class SettingData : Resource
{
 [Export] public string sID;
 [Export] public string sLanguage;
 [Export] public string sForwardButton;
 [Export] public string sBackwardButton;
 [Export] public string sLeftButton;
 [Export] public string sRightButton;
 [Export] public string sJumpButton;
 [Export] public string sAimButton;
 [Export] public string sUseButton;
 [Export] public string sInteractButton;
 [Export] public float fMasterV;
 [Export] public float fBGMV;
 [Export] public float fSFXV;
 [Export] public float fVoicesV;
 [Export] public float fFrameLimit;
 [Export] public float fMouseSensitivity;
 [Export] public float fFOV;
 [Export] public int iWindowModeIndex;
 [Export] public int iGameResIndex;
 [Export] public bool bVSyncEnabled;
 [Export] public bool bInvertedControls;
 
 public void LangInit(string LanguageCode)
 {
  
  sLanguage = LanguageCode;
  TranslationServer.SetLocale(LanguageCode);

 }

 public void ScreenWindowinit(int x)
 {
  if (x == 0)
  {
   DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
  }
  else if (x == 1)
  {
   DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
   DisplayServer.WindowSetSize(new Vector2I(1280, 720));
  }
 }

 public void SetAudioSetting(float m, int idx)
  {

   if (idx == 0)
   {
    fMasterV = m;
    AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(m));
   }
   else if (idx == 1)
   {
    fBGMV = m;
    AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb(m));
   }
   else if (idx == 2)
   {
    fSFXV = m;
    AudioServer.SetBusVolumeDb(2, Mathf.LinearToDb(m));
   }else if (idx == 3)
   {
    fVoicesV = m;
    AudioServer.SetBusVolumeDb(3, Mathf.LinearToDb(m));
   }

  }
 
  public void SetFOV(double y)
  {

   fFOV = (float)y;
  }
  private void SetMouseSensitivity(double y)
  {

   fMouseSensitivity = (float)y;
  }
}


