using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private float fRunMultiplier = 1.5f;
	[Export] private float fMoveMentSpeed = 5.0f;
	[Export] private float fJumpForce = 5.0f;
	[Export] private Camera3D PlayerCamera;
    private float MousePositionX;
    private float MousePositionY;
	private Vector2 MoveMentDirection;
	private Vector3 MoveMentDirectionTranslation;
	[Export] private Node3D YawNode;
	[Export] private Node3D PitchNode;
	private Vector3 CalcVelocity;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		CalcVelocity = Velocity;
		if (!IsOnFloor())
		{
			CalcVelocity += GetGravity() * (float)delta;
		}

		if (IsOnFloor()&& Input.IsActionPressed("Jump"))
		{
			CalcVelocity.Y = fJumpForce;
		}
		MoveMentDirection = Input.GetVector("Move_Left", "Move_Right","Move_Backward", "Move_Forward");
		//MoveMentDirectionTranslation = (Transform.Basis * new Vector3(MoveMentDirection.X, 0, -MoveMentDirection.Y)).Normalized();
		
		MoveMentDirectionTranslation = MoveMentDirection.X * YawNode.Basis.X - MoveMentDirection.Y * YawNode.Basis.Z;
		MoveMentDirectionTranslation.Y = 0;
		
		if (MoveMentDirectionTranslation != Vector3.Zero)
		{

			CalcVelocity.X = MoveMentDirectionTranslation.X * fMoveMentSpeed;
			CalcVelocity.Z = MoveMentDirectionTranslation.Z * fMoveMentSpeed;
			if (Input.IsActionPressed("Run"))
			{
				CalcVelocity.X = MoveMentDirectionTranslation.X * fMoveMentSpeed * fRunMultiplier;
				CalcVelocity.Z = MoveMentDirectionTranslation.Z * fMoveMentSpeed * fRunMultiplier;
			}

		}
		else
		{
			CalcVelocity.X = 0.0f;
			CalcVelocity.Z = 0.0f;
		}
		
		 Velocity = CalcVelocity;
		MoveAndSlide();
		

	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			
			Vector2 MousePosC = eventMouseMotion.Relative/300 * Mathf.Pi;
			
			YawNode.RotateY(-MousePosC.X);
			float clampedPitch = Mathf.Clamp(PitchNode.Rotation.X - MousePosC.Y, -Mathf.Pi / 2, Mathf.Pi / 2); 
			PitchNode.Rotation = new Vector3(clampedPitch, PitchNode.Rotation.Y, PitchNode.Rotation.Z); 
			
			
			MousePositionX -= eventMouseMotion.Relative.X;
			//MousePositionY -= eventMouseMotion.Relative.Y*0.5f;
			//MousePositionY = Mathf.Clamp(MousePositionY, -89f, 89f);
			//PlayerCamera.GlobalRotationDegrees = new Vector3(_pitch, _yaw, 0);
			
		}
	}
}
