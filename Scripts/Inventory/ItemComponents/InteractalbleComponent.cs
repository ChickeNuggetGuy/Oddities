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


    
	public void Interact(Player player, WorldItem parent)
	{
		switch (InteractionBehavior)
		{
			case Enums.InteractionBehavior.PICKUP:
				if (player != null)
				{
					player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory);
					
					if (playerInventory.mainInventory.TryAddItem(parent.itemData, parent.stackSize))
					{
						parent.QueueFree();
					}
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
				
				WorldItem worldItem = InventoryManager.InstantiateWorldItem(parent, 1);
				player.GetTree().Root.AddChild(worldItem);
				if (!playerInteraction.TryHoldWorldItem(worldItem))
				{
					return;
				}
				break;
			
		}
	}
}
