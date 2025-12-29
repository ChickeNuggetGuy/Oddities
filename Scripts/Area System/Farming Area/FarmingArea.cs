using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class FarmingArea : AllowedArea
{

	public override void _Ready()
	{
		base._Ready();
		GD.Print($"FarmingArea initialized with {validPoints.Count} potential spots.");
	}

	public void PlantAtSpot(Vector3 spot, ItemData plantInstance, Player player)
	{
		if (validPoints.Contains(spot) && !occupiedSpots.ContainsKey(spot))
		{
			WorldItem worldItem = InventoryManager.Instance.InstantiateWorldItem(plantInstance, 1);
			if (!worldItem.TryGetItemComponent<PlantableComponent>(out PlantableComponent plantableComponent))
			{
				//Item cannot be planted, return
				return;
			}
			
			if (!player.TryGetPlayerComponent<PlayerStatHolder>(out PlayerStatHolder playerStatHolder) &&
			    plantableComponent.plantingCost.Count > 0)
			{
				return;
			}

			if (!playerStatHolder.TrySpendStatCost(plantableComponent.plantingCost))
			{
				return;
			}


			occupiedSpots[spot] = worldItem;
			AddChild(worldItem);
			worldItem.PickedUp += WorldItemOnPickedUp;
			worldItem.Freeze = true;
			worldItem.GlobalPosition = spot;
			
			
			if (worldItem.TryGetItemComponent<InteractalbleComponent>(out InteractalbleComponent interactalbleComponent))
			{
				interactalbleComponent.InteractionBehavior = Enums.InteractionBehavior.HARVEST;
			}

			worldItem.itemMesh.Mesh = plantableComponent.GetGrowthStage(0).StageMesh;

		}
	}

	private void WorldItemOnPickedUp(WorldItem item, Vector3 position)
	{
		if (occupiedSpots.ContainsKey(position))
		{
			occupiedSpots.Remove(position);
		}
		else
		{
			GD.Print("Occupied spot not found");
		}
		item.PickedUp -= WorldItemOnPickedUp;
	}
}