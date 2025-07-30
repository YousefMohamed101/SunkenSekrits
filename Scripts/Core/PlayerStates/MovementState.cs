using Godot;
using System;

public partial class MovementState : State {
	// Called when the node enters the scene tree for the first time.
	public override void Ready() { GD.Print("MovementState.Ready"); }

	public override void Enter() {
		Manager.PlayerController.BobbingSpeed = 10.0f;
		Manager.PlayerController.BobbingAmplitude = 0.1f;
		Manager.PlayerController.SwaySpeed = 5.0f;
		Manager.PlayerController.SwayAmplitude = 0.05f;
		Manager.PlayerController.SpeedMultiplier = 1.5f;
		Manager.PlayerController.PlayerCamera.SetPosition(Vector3.Up);
		GD.Print("MovementState.Enter");
	}

	public override void ProcessUpdate(float delta) {
		Manager.PlayerController.Timer += delta;
		if(Manager.PlayerController.MovementDirectionTranslation != Vector3.Zero) {
			float _targetVoffset = Mathf.Sin(Manager.PlayerController.Timer * Manager.PlayerController.BobbingSpeed) * Manager.PlayerController.BobbingAmplitude;
			float _targetHoffset = Mathf.Sin(Manager.PlayerController.Timer * Manager.PlayerController.SwaySpeed) * Manager.PlayerController.SwayAmplitude;
			;

			Manager.PlayerController.PlayerCamera.VOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.VOffset, _targetVoffset, 5.0f * delta);
			Manager.PlayerController.PlayerCamera.HOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.HOffset, _targetHoffset, 5.0f * delta);

			if(Input.IsActionPressed("Run_Movement")) {
				Manager.PlayerController.PlayerCamera.VOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.VOffset, _targetVoffset, 5.0f * Manager.PlayerController.SpeedMultiplier * delta);
				Manager.PlayerController.PlayerCamera.HOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.HOffset, _targetHoffset, 5.0f * Manager.PlayerController.SpeedMultiplier * delta);
			}
		} else {
			Manager.PlayerController.PlayerCamera.VOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.VOffset, Manager.PlayerController.DefaultBsAmplitude, 0.1f * (float)delta);
			Manager.PlayerController.PlayerCamera.HOffset = Mathf.Lerp(Manager.PlayerController.PlayerCamera.HOffset, Manager.PlayerController.DefaultBsAmplitude, 0.3f * (float)delta);
			Manager.PlayerController.Timer = 0.0f;
		}
	}

	public override void PhysicsUpdate(float delta) {
		if(!Manager.PlayerController.IsOnFloor()) {
			Manager.PlayerController.CalcVelocity.Y += Manager.PlayerController.GetGravity().Y * delta * Manager.PlayerController.FallSpeed;
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