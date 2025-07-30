using Godot;
using System;

public partial class Swimming : State {
	// Called when the node enters the scene tree for the first time.
	public override void Ready() { GD.Print("SwimmingState.Ready"); }

	public override void Enter() {
		Manager.PlayerController.SpeedMultiplier = 1.5f;
		Manager.PlayerController.PlayerCamera.SetPosition(Vector3.Up);
		GD.Print("SwimmingState.Enter");
	}

	public override void Exit() { GD.Print("SwimmingState.Exit"); }

	public override void PhysicsUpdate(float delta) {
		if(Input.IsActionPressed("Jump_Movement")) {
			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.SwimmingSpeed;
			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController.CalcVelocity.Y *= Manager.PlayerController.SpeedMultiplier;
			}
		} else if(Input.IsActionPressed("Crouch_Movement")) {
			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.SwimmingSpeed * -1.0f;
			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController.CalcVelocity.Y *= Manager.PlayerController.SpeedMultiplier;
			}
		} else {
			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.GetGravity().Y * delta;
		}

		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController.CalcVelocity.X = Manager.PlayerController.MovementDirectionTranslation.X * Manager.PlayerController.MovementSpeed * delta;
			Manager.PlayerController.CalcVelocity.Z = Manager.PlayerController.MovementDirectionTranslation.Z * Manager.PlayerController.MovementSpeed * delta;

			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController.CalcVelocity.X *= Manager.PlayerController.SpeedMultiplier;
				Manager.PlayerController.CalcVelocity.Z *= Manager.PlayerController.SpeedMultiplier;
			}
		} else {
			Manager.PlayerController.CalcVelocity.X = 0;
			Manager.PlayerController.CalcVelocity.Z = 0;
		}

		Manager.PlayerController.Velocity = Manager.PlayerController.CalcVelocity;
		Manager.PlayerController.MoveAndSlide();
	}
}