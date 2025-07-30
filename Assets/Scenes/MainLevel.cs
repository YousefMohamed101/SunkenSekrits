using Godot;
using System;

public partial class MainLevel : Node3D {
	// Called when the node enters the scene tree for the first time.
	private WorldEnvironment _worldEnvironment;

	public override void _Ready() { _worldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment"); }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var skyrotation = _worldEnvironment.Environment.GetSkyRotation();
		skyrotation.Y += (float)delta * 0.001f;
		_worldEnvironment.Environment.SetSkyRotation(skyrotation);
	}
}