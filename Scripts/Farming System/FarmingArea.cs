using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class FarmingArea : AllowedArea
{

	public override void _Ready()
	{
		// Call base to generate points
		base._Ready();
		GD.Print($"FarmingArea initialized with {validPoints.Count} potential spots.");
	}

	public void PlantAtSpot(Vector3 spot, Node3D plantInstance)
	{
		if (validPoints.Contains(spot) && !occupiedSpots.ContainsKey(spot))
		{
			occupiedSpots[spot] = plantInstance;
			AddChild(plantInstance);
			plantInstance.GlobalPosition = spot;
		}
	}
}