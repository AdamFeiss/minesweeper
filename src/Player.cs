using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float mouse_sensitivity = 0.01f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public Node3D playerNeck;
	public Camera3D playerCamera;

	public override void _Ready()
	{
		playerNeck = GetNode<Node3D>("PlayerCameraPivotPoint");
		playerCamera = GetNode<Camera3D>("PlayerCameraPivotPoint/PlayerCamera");
	}

	// Overrides Standard "UnhandledInput" method
	public override void _UnhandledInput(InputEvent @event)
	{
		MouseCapture(@event);
		PlayerPOVRotation(@event);
	}

	// Handle the capturing of the mouse and the release with ESC
	public static void MouseCapture(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	// Rotates the Players Point of View by using the mouse
	public void PlayerPOVRotation(InputEvent @event)
	{
		// Check if the mouse is beeing captured
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion eventMouseButton)
			{
				// Rotates neck horizontally and camera vertically
				playerNeck.RotateY(-eventMouseButton.Relative.X * mouse_sensitivity);
				playerCamera.RotateX(-eventMouseButton.Relative.Y * mouse_sensitivity);

				// Checks if the camera is between -30 degree and 60 vertical rotation and clamps it between those values
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
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");

		// Get the relative Viewdirection from the Player POV
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
