using Godot;
using System;

public partial class EntityDebuger : Control {
	// Called when the node enters the scene tree for the first time.

	[Export] public Label Namelbl {get; set;}
	[Export] public Label Speedlbl {get; set;}
	[Export] public Player Player {get; set;}

	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Namelbl.Text = "Height: " + Player.GlobalPosition.Y;
		Speedlbl.Text = "Speed: " + MathF.Sqrt(MathF.Pow(Player.Velocity.X, 2) + MathF.Pow(Player.Velocity.Z, 2));
	}
}