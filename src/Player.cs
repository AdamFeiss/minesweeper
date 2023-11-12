using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	public Node3D playerNeck;
	public Camera3D playerCamera;
	
	public override void _Ready()
	{
		playerNeck = GetNode<Node3D>("PlayerCameraPivotPoint");
		playerCamera = GetNode<Camera3D>("PlayerCameraPivotPoint/PlayerCamera");
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion eventMouseButton)
			{
				playerNeck.RotateY(-eventMouseButton.Relative.X * 0.01f);
				playerCamera.RotateX(-eventMouseButton.Relative.Y * 0.01f);

				Vector3 currentRotation = playerCamera.RotationDegrees;
				float newRotationX = Mathf.Clamp(currentRotation.X, -30, 60);

				playerCamera.RotationDegrees = new Vector3(newRotationX, currentRotation.Y, currentRotation.Z);

			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (playerNeck.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
