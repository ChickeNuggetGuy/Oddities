using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class InteractalbleComponent : ItemComponent, IInteractable
{
	[Export] public Key InteractionKey { get; set; } = Key.E;
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
		if (!player.TryGetPlayerComponent<PlayerStatHolder>(out var playerStatHolder))
		{
			return;
		}
		switch (InteractionBehavior)
		{
			case Enums.InteractionBehavior.HARVEST:
				if (parent.TryGetItemComponent<PlantableComponent>(out var plantable))
				{
					if (plantable.IsMature)
					{
						Dictionary<ItemData, Variant> result = plantable.GetHarvestResult(out var growthStage);

						if (!playerStatHolder.TrySpendStatCost(growthStage.harvestCost))
						{
							return;
						}
			
						if (result != null && result.Count > 0)
						{
							foreach (var itemKVP in result)
							{
								int amount = 1;
								switch (itemKVP.Value.VariantType)
								{
									case Variant.Type.Int:
										amount = itemKVP.Value.AsInt32();
										break;
									case Variant.Type.Vector2I:
										Vector2I range = itemKVP.Value.AsVector2I();
										amount = GD.RandRange(range.X, range.Y);
										break;
									default:
										GD.Print("Variant type not implemented");
										continue;
								}
								
								if (!parent.TryGiveItemToPlayer(player, itemKVP.Key, amount))
								{
									//Pickup failed, drop item on floor
									//TODO: Drop item on floor
								}
								
							}
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

					parent.TryGiveItemToPlayer(player, parent.itemData);

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