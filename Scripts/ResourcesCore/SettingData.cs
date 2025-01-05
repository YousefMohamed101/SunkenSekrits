using Godot;

[GlobalClass]
public partial class SettingData : Resource {
	[Export] public string Id;
	[Export] public string ForwardButton;
	[Export] public string BackwardButton;
	[Export] public string LeftButton;
	[Export] public string RightButton;
	[Export] public string JumpButton;
	[Export] public string AimButton;
	[Export] public string UseButton;
	[Export] public string InteractButton;
	[Export] public float MasterVolume;
	[Export] public float BgmVolume;
	[Export] public float SfxVolume;
	[Export] public float VoicesVolume;
	[Export] public float FrameLimit;
	[Export] public float MouseSensitivity;
	[Export] public float Fov;
	[Export] public int Language;
	[Export] public int WindowModeIndex;
	[Export] public int GameResIndex;
	[Export] public int VSync;
	[Export] public bool InvertedMode;


	public void ScreenWindowinit(int x) {
		if(x == 0) {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		} else if(x == 1) {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			DisplayServer.WindowSetSize(new Vector2I(1280, 720));
		}
	}
}