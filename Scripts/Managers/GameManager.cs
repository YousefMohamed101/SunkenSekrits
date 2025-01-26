using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance;
	
	[Signal] public delegate void MouseSenseChangedEventHandler(float fov);
	[Signal] public delegate void FOVChangedEventHandler(float fov);

	public bool gamestate;
	public bool gamepaused;
	
	
	
	public override void _Ready() {
		gamestate = false;
		gamepaused = false;
		Instance = this;

	}

}
