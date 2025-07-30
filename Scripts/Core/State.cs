using Godot;
using System;

public partial class State : Node {
	public StateMachineManager Manager;

	// Called when the node enters the scene tree for the first time.
	public virtual void Enter() {}

	public virtual void Ready() {}
	public virtual void ProcessUpdate(float delta) {}

	public virtual void PhysicsUpdate(float delta) {}

	public virtual void HandleInput(InputEvent @event) {}

	public virtual void Exit() {}
}