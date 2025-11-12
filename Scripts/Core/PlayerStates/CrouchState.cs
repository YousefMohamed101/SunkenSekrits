using Godot;
using System;

public partial class CrouchState : State {
	public override void Ready() { GD.Print("CrouchState.Ready"); }

	public override void Enter() {
		Manager.PlayerController.SpeedMultiplier = 0.4f;
		Manager.PlayerController.CameraAnchoring.Position = Vector3.Up * 0.5f * 1.8f;
		GD.Print("CrouchState.Enter");
	}

	public override void Exit() {
		Manager.PlayerController.CameraAnchoring.Position = Vector3.Up * 1.8f;
		Manager.PlayerController.SpeedMultiplier = 1.5f;
		GD.Print("CrouchState.Exit");
	}


	public override void ProcessUpdate(float delta) {
		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 10.0f, 0.03f, 5.0f, 0.03f, 5.0f, 1.0f);
		} else {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 0, 0, 0, 0, 0.1f, 1.0f);
			Manager.PlayerController.Timer = 0.0f;
		}
	}

	public override void PhysicsUpdate(float delta) {
		if(!Manager.PlayerController.IsOnFloor()) {
			Manager.PlayerController.CalcVelocity.Y += Manager.PlayerController.GetGravity().Y * delta * Manager.PlayerController.FallSpeed * 0.5f;
		}

		if(Manager.PlayerController.IsOnFloor() && Input.IsActionPressed("Jump_Movement")) {
			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.JumpForce;
			GD.Print("Jumped");
		}

		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController.CalcVelocity.X = Manager.PlayerController.MovementDirectionTranslation.X * Manager.PlayerController.MovementSpeed * Manager.PlayerController.SpeedMultiplier * delta;
			Manager.PlayerController.CalcVelocity.Z = Manager.PlayerController.MovementDirectionTranslation.Z * Manager.PlayerController.MovementSpeed * Manager.PlayerController.SpeedMultiplier * delta;
		} else {
			Manager.PlayerController.CalcVelocity.X = 0;
			Manager.PlayerController.CalcVelocity.Z = 0;
		}

		Manager.PlayerController.Velocity = Manager.PlayerController.CalcVelocity;
		Manager.PlayerController.MoveAndSlide();
		if(!Manager.PlayerController.IsFootstepActive()) {
			Manager.PlayerController.PlayFootstepSound();
		}

		GodotObject collider = Manager.PlayerController.RayCast.GetCollider();
		if(collider is Interactable i) {
			Manager.PlayerController.CurrentInteractable = i;
			Manager.PlayerController.HudLayer.InteractLabel.Visible = true;
		} else {
			Manager.PlayerController.CurrentInteractable = null;
			Manager.PlayerController.HudLayer.InteractLabel.Visible = false;
		}
	}


	public override void HandleInput(InputEvent @event) {
		if(Input.IsActionPressed("Crouch_Movement")) {
			Manager.TransitionToState("Movement");
		}

		if(Input.IsActionJustPressed("Interact_Action") && Manager.PlayerController.CurrentInteractable != null) {
			Manager.PlayerController.CurrentInteractable.Interact();
			GetViewport().SetInputAsHandled();
		}
	}
}