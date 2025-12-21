using Godot;
using System;

[GlobalClass]
public partial class Bed : Node3D
{
	[Export] public Area3D area { get; set; }


	public override void _Ready()
	{
		base._Ready();
		
		area.BodyEntered += AreaOnBodyEntered;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		area.BodyEntered -= AreaOnBodyEntered;
	}

	private void AreaOnBodyEntered(Node3D body)
	{

		if (body.IsInGroup("Player"))
		{
			DayManager.Instance.TryAdvanceDay();
		}
	}
}
