using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SettingData : Resource {
	[Export] public string Id;
	[Export] public InputEvent ForwardButton;
	[Export] public InputEvent BackwardButton;
	[Export] public InputEvent LeftButton;
	[Export] public InputEvent RightButton;
	[Export] public InputEvent JumpButton;
	[Export] public InputEvent RunButton;
	[Export] public InputEvent CrouchButton;
	[Export] public InputEvent InteractButton;
	[Export] public InputEvent AimButton;
	[Export] public InputEvent UseButton;
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