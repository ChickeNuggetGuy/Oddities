using Godot;
using System;

[GlobalClass]
public partial class PlayerInventory : PlayerComponent
{
	[Export] public Inventory mainInventory;
	public int selectedItemIndex = 0;


	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!mainInventory.TryGetItemAtIndex(selectedItemIndex, out var itemData))
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				if (parentPlayer.CurrentAreas.Count > 0)
				{
					if (!parentPlayer.TryGetPlayerComponent<PlayerInteraction>(out var playerInteraction)) return;

					// Perform the raycast once
					var result = playerInteraction.PerformRaycast(10);
					if (!result.TryGetValue("position", out var position)) return;
					Vector3 hitPos = (Vector3)position;

					foreach (var area in parentPlayer.CurrentAreas)
					{
						if (area is not FarmingArea farmingArea) continue;
						
						if (!itemData.TryGetItemComponent<PlantableComponent>(out var plantableComponent)) continue;
						Vector3? nearestAvailable = farmingArea.GetNearestAvailableSpot(hitPos);

						if (nearestAvailable.HasValue)
						{
							farmingArea.PlantAtSpot(nearestAvailable.Value, itemData);
							
							mainInventory.TryRemoveItem(itemData);
							break;
						}
					}
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
			{
				if (!itemData.TryGetItemComponent<InteractalbleComponent>(out var interactableComponent)) return;
				interactableComponent.Interact(parentPlayer, itemData);
			}
		}
	}
}