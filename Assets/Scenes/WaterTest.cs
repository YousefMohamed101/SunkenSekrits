using Godot;
using System;

public partial class WaterTest : Area3D {
	public override void _Ready() {
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body) {
		if(body is Player p) {
			p.Manager.TransitionToState("Swimming");
			GD.Print("Water entered");
		}
	}

	private void OnBodyExited(Node body) {
		if(body is Player p) {
			p.Manager.TransitionToState("Movement");
			p.SetVelocity(new Vector3(p.Velocity.X, 10, p.Velocity.Z));
			GD.Print("Water exited");
		}
	}
}