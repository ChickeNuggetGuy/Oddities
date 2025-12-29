using Godot;
using System;

[GlobalClass]
public partial class GameManager : Manager<GameManager>
{
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
				GD.Print($"setting up manager: {manager.Name} ");
				manager.SetupCall();
				GD.Print($"Successfully setup manager: {manager.Name} ");
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
