using Godot;
using System;

[GlobalClass]
public partial class SettingData : Resource
{
 [Export] public string sID;
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
 [Export] public int iLanguage;
 [Export] public int iWindowModeIndex;
 [Export] public int iGameResIndex;
 [Export] public int iVSync;
 [Export] public bool bInvertedControls;
 
 
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

}


