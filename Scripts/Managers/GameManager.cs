using Godot;
using System;

public partial class GameManager : Node {
	[Signal]
	public delegate void FovChangedEventHandler(float fov);

	[Signal]
	public delegate void GameOutofFocusEventHandler();

	[Signal]
	public delegate void MouseSenseChangedEventHandler(float fov);

	[Signal]
	public delegate void RideCarEventHandler(Car car);

	public static GameManager Instance;
	public bool gamepaused;
	public bool gamestate;

	/*
	public override void _Notification(int what) {
		if(what == NotificationApplicationFocusOut) {
			gamepaused = true;
			GetTree().Paused = true;
			EmitSignalGameOutofFocus();
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GD.Print("Game paused");
		}
	}
	*/

	public override void _Ready() {
		gamestate = false;
		gamepaused = false;
		Instance = this;
	}
}