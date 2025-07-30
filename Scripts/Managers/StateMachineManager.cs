using Godot;
using System;
using Godot.Collections;

public partial class StateMachineManager : Node {
	private State _currentState;

	private Dictionary<string, State> _states = new Dictionary<string, State>();

	public Player PlayerController;
	// Called when the node enters the scene tree for the first time.

	[Export] public NodePath InitialState {get; set;}

	public override void _Ready() {
		PlayerController = GetParent<Player>();

		var stateChildren = GetChildren();

		foreach(var child in stateChildren) {
			if(child is State s) {
				_states.Add(child.Name, s);
				s.Manager = this;
				s.Ready();
				s.Exit();
			}
		}

		_currentState = GetNode<State>(InitialState);
		_currentState.Enter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) { _currentState.ProcessUpdate((float)delta); }

	public override void _PhysicsProcess(double delta) { _currentState.PhysicsUpdate((float)delta); }

	public override void _Input(InputEvent @event) { _currentState.HandleInput(@event); }

	public void TransitionToState(string newState) {
		if(_states[newState] == _currentState || !_states.ContainsKey(newState)) {
			return;
		}

		_currentState.Exit();
		_currentState = _states[newState];
		_currentState.Enter();
	}
}