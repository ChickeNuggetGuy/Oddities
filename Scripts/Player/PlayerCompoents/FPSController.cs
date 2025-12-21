using Godot;
using System;

[GlobalClass]
public partial class FPSController : PlayerComponent
{
    [ExportGroup("Movement Settings")]
    [Export] private float _speed = 5.0f;
    [Export] private float _jumpVelocity = 4.5f;
    [Export] private float _mouseSensitivity = 0.002f;

    [ExportGroup("Nodes")]
    [Export] private Node3D _headPivot;
    [Export] private Camera3D _camera;

    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (parentPlayer == null) return;

        if (@event is InputEventMouseMotion mouseMotion)
        {
            // 2. Rotate the Player Root (Horizontal - Y Axis)
            parentPlayer.RotateY(-mouseMotion.Relative.X * _mouseSensitivity);

            // 3. Rotate the Head Pivot (Vertical - X Axis)
            Vector3 headRotation = _headPivot.Rotation;
            headRotation.X -= mouseMotion.Relative.Y * _mouseSensitivity;
            
            headRotation.X = Mathf.Clamp(
                headRotation.X, 
                Mathf.DegToRad(-89f), 
                Mathf.DegToRad(89f)
            );
            
            _headPivot.Rotation = headRotation;
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
	    if (parentPlayer == null) return;

	    Vector3 velocity = parentPlayer.Velocity;


	    if (!parentPlayer.IsOnFloor())
	    {
		    velocity.Y -= _gravity * (float)delta;
	    }
	    
	    if (Input.IsActionJustPressed("jump") && parentPlayer.IsOnFloor())
	    {
		    velocity.Y = _jumpVelocity;
	    }

	    Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveUp", "moveDown");
	    
	    Vector3 direction = (parentPlayer.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

	    if (direction != Vector3.Zero)
	    {
		    velocity.X = direction.X * _speed;
		    velocity.Z = direction.Z * _speed;
	    }
	    else
	    {
		    // Smoothly slow down when no input is provided
		    velocity.X = Mathf.MoveToward(parentPlayer.Velocity.X, 0, _speed);
		    velocity.Z = Mathf.MoveToward(parentPlayer.Velocity.Z, 0, _speed);
	    }

	    parentPlayer.Velocity = velocity;
	    parentPlayer.MoveAndSlide();
    }
}