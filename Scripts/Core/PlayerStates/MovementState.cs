using Godot;
using System;

public partial class MovementState : State {
	// Called when the node enters the scene tree for the first time.
	public override void Ready() { GD.Print("MovementState.Ready"); }

	public override void Enter() {
		Manager.PlayerController.CameraAnchoring.Position = Vector3.Up * 1.8f;
		Manager.PlayerController.AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["Walking"]);
	}

	public override void ProcessUpdate(float delta) {
		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 10.0f, 0.03f, 5.0f, 0.03f, 5.0f, 1.0f);

			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController._cameraController.BobSwayControll(delta, 5.0f, 0.04f, 2.5f, 0.04f, 5.0f, 0.9f);
			}
		} else {
			Manager.PlayerController._cameraController.BobSwayControll(delta, 0, 0, 0, 0, 0.1f, 1.0f);
			Manager.PlayerController.Timer = 0.0f;
		}
	}

	public override void PhysicsUpdate(float delta) {
		if(!Manager.PlayerController.IsOnFloor()) {
			Manager.PlayerController.CalcVelocity.Y += Manager.PlayerController.GetGravity().Y * delta * Manager.PlayerController.FallSpeed;
		}

		if(Manager.PlayerController.IsOnFloor() && Input.IsActionPressed("Jump_Movement")) {
			Manager.PlayerController.StopFootstepSound();
			Manager.PlayerController.AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["JumpingStart"]);
			Manager.PlayerController.PlayFootstepSound();


			Manager.PlayerController.CalcVelocity.Y = Manager.PlayerController.JumpForce;
			GD.Print("Jumped");
		}

		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			Manager.PlayerController.CalcVelocity.X = Manager.PlayerController.MovementDirectionTranslation.X * Manager.PlayerController.MovementSpeed * delta;
			Manager.PlayerController.CalcVelocity.Z = Manager.PlayerController.MovementDirectionTranslation.Z * Manager.PlayerController.MovementSpeed * delta;
			if(!Manager.PlayerController.IsFootstepActive() && Manager.PlayerController.IsOnFloor()) {
				Manager.PlayerController.AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["Walking"]);
				Manager.PlayerController.PlayFootstepSound();
			}

			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController.CalcVelocity.X *= Manager.PlayerController.SpeedMultiplier;
				Manager.PlayerController.CalcVelocity.Z *= Manager.PlayerController.SpeedMultiplier;
				if(!Manager.PlayerController.IsFootstepActive() && Manager.PlayerController.IsOnFloor()) {
					Manager.PlayerController.AssignFootStepStreams(DataBaseManager.Instance.StreamLibrary["Normal"]["Running"]);
					Manager.PlayerController.PlayFootstepSound();
				}
			}
		} else {
			Manager.PlayerController.CalcVelocity.X = 0;
			Manager.PlayerController.CalcVelocity.Z = 0;
			if(Manager.PlayerController.IsFootstepActive()) {
				Manager.PlayerController.StopFootstepSound();
			}
		}

		Manager.PlayerController.Velocity = Manager.PlayerController.CalcVelocity;
		Manager.PlayerController.MoveAndSlide();
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
			Manager.TransitionToState("Crouch");
		}

		if(Input.IsActionJustPressed("Interact_Action") && Manager.PlayerController.CurrentInteractable != null) {
			Manager.PlayerController.CurrentInteractable.Interact();
			GetViewport().SetInputAsHandled();
		}
	}

	public override void Exit() { GD.Print("MovementState.Exit"); }
}