using Godot;
using System;

[GlobalClass]
public partial class GameManager : Manager<GameManager>
{
	public static GameManager Instance { get; protected set; }
	
	public override void _EnterTree()
	{
		base._EnterTree();
		if (Instance != null)
		{
			if (Instance != this)
			{
				GD.PrintErr("Only one instance of UIManager allowed, destroying self");
				this.QueueFree();
			}
		}
		else
		{
			Instance = this;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		SetupCall();
	}

	protected override void Setup()
	{
		foreach (var child in GetChildren())
		{
			if (child is ManagerBase manager)
			{
				manager.SetupCall();
			}
		}
		return;
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	base._Input(@event);
	// 	if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
	// 	{
	// 		GetTree().Quit();
	// 	}
	// }
}
