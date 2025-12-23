using Godot;
using System;

[GlobalClass]
public partial class InteractalbleComponent : ItemComponent, IInteractable
{
	[Export] public Key InteracionKey { get; set; } = Key.E;
	[Export] public Enums.InteractionBehavior InteractionBehavior = Enums.InteractionBehavior.PICKUP;

	public override void Initialize()
	{
		return;
	}

	public override void Terminate()
	{
	}


	public void Interact(Player player, WorldItem parent)
	{
		switch (InteractionBehavior)
		{
			case Enums.InteractionBehavior.HARVEST:
				if (parent.TryGetItemComponent<PlantableComponent>(out var plantable))
				{
					if (plantable.IsMature)
					{
						ItemData result = plantable.GetHarvestResult();
						if (result != null)
						{
							parent.Pickup(player, result);
						}
					}
					else
					{
						GD.Print("This plant is not ready for harvest yet.");
					}
				}

				break;
			case Enums.InteractionBehavior.PICKUP:
				if (player != null)
				{
					player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory);

					parent.Pickup(player, parent.itemData);

				}

				break;
			case Enums.InteractionBehavior.USE:

				if (!parent.TryGetItemComponent<StatAffectedComponent>(out var statAffectedComponent))
				{
					GD.Print("StatAffectedComponent not found");
					return;
				}

				statAffectedComponent.AffectStats(player);

				break;
			case Enums.InteractionBehavior.DELETE:
				ParentWorldItem.QueueFree();
				break;
			case Enums.InteractionBehavior.HOLD:

				if (!player.TryGetPlayerComponent<PlayerInteraction>(out var playerInteraction)) return;
				if (playerInteraction.CurrentlyHeldItem != null)
				{
					return;
				}

				if (!playerInteraction.TryHoldWorldItem(parent))
				{
					return;
				}

				break;
		}
	}

	public void Interact(Player player, ItemData parent)
	{
		switch (InteractionBehavior)
		{
			case Enums.InteractionBehavior.USE:

				if (!parent.TryGetItemComponent<StatAffectedComponent>(out var statAffectedComponent))
				{
					GD.Print("StatAffectedComponent not found");
					return;
				}

				statAffectedComponent.AffectStats(player);

				break;
			case Enums.InteractionBehavior.DELETE:
				ParentWorldItem.QueueFree();
				break;
			case Enums.InteractionBehavior.HOLD:
				//TODO: Spawn Item in world to hold


				if (!player.TryGetPlayerComponent<PlayerInteraction>(out var playerInteraction)) return;
				if (playerInteraction.CurrentlyHeldItem != null)
				{
					return;
				}

				WorldItem worldItem = InventoryManager.Instance.InstantiateWorldItem(parent, 1);
				player.GetTree().Root.AddChild(worldItem);
				if (!playerInteraction.TryHoldWorldItem(worldItem))
				{
					return;
				}

				break;
		}
	}
}