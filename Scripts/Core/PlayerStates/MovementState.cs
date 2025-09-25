using Godot;
using System;

public partial class MovementState : State {
	// Called when the node enters the scene tree for the first time.
	public override void Ready() { GD.Print("MovementState.Ready"); }

	public override void Enter() { Manager.PlayerController.CameraAnchoring.Position = Vector3.Up * 1.8f; }

	public override void ProcessUpdate(float delta) {
		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 10.0f, 0.1f, 5.0f, 0.05f, 5.0f, 1.0f);

			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController._cameraController.BobSwayControll(delta, 10.0f, 0.1f, 5.0f, 0.05f, 5.0f, 1.5f);
			}
		} else {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 0, 0, 0, 0, 0.1f, 1.0f);
			Manager.PlayerController.Timer = 0.0f;
		}
	}

	public override void PhysicsUpdate(float delta) {
		if(!Manager.PlayerController.IsOnFloor()) {
			Manager.PlayerController.CalcVelocity.Y += Manager.PlayerController.GetGravity().Y * delta;
			GD.Print(Manager.PlayerController.Position);
		}

		if(Manager.PlayerController.IsOnFloor() && Input.IsActionPressed("Jump_Movement")) {
			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.JumpForce;
			GD.Print("Jumped");
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

	public override void HandleInput(InputEvent @event) {
		if(Input.IsActionPressed("Crouch_Movement")) {
			Manager.TransitionToState("Crouch");
		}
	}

	public override void Exit() { GD.Print("MovementState.Exit"); }
}