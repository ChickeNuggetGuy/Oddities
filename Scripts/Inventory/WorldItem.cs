using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class WorldItem : RigidBody3D
{
	[Export] public ItemData itemData;
	[Export] public int stackSize = 1;
	[Export] public MeshInstance3D itemMesh;

	#region Signals
	[Signal] public delegate void PickedUpEventHandler(WorldItem item, Vector3 position);
	#endregion
	public WorldItem()
	{
		itemData = null;
	}
	
	public WorldItem(ItemData data, int stack = 1)
	{
		itemData = data;
		stackSize = stack;
	}

	public void Initialize()
	{
		if (itemMesh == null)
		{
			itemMesh = new MeshInstance3D();
			AddChild(itemMesh);
		}

		if (itemData.itemMesh != null)
		{
			itemMesh.Mesh = itemData.itemMesh;
		}
		
		foreach (ItemComponent component in itemData.components)
		{
			component.InitializeCall(this);
		}
	}

	public override void _ExitTree()
	{
		foreach (ItemComponent component in itemData.components)
		{
			component.TerminateCall(this);
		}
		base._ExitTree();
	}

	public bool TryGetItemComponent<T>(out T component) where T : ItemComponent
	{
		foreach (ItemComponent itemComponent in itemData.components)
		{
			if (itemComponent is T)
			{
				component = itemComponent as T;
				return true;
			}
		}
		component = null;
		return false;
	}


	public void OnInteracted(Player player)
	{
		// Simply find the interactable component and let it handle the logic
		if (TryGetItemComponent<InteractalbleComponent>(out var interactable))
		{
			interactable.Interact(player, this);
		}
	}
	
	public bool TryGiveItemToPlayer(Player player,ItemData data, int amount = -1)
	{
		if (!player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory)) return false;
		
		if (playerInventory.inventories[Enums.InventoryType.HOTBAR].TryAddItem(data,amount != -1 ? amount : stackSize))
		{
			GD.Print($"Harvested: {data.ItemName}");
			EmitSignal(SignalName.PickedUp, this, this.GlobalPosition);
			this.QueueFree();
			return true;
		}
		else if (playerInventory.inventories[Enums.InventoryType.MAIN].TryAddItem(data,amount != -1 ? amount : stackSize))
		{
			GD.Print($"Harvested: {data.ItemName}");
			EmitSignal(SignalName.PickedUp, this, this.GlobalPosition);
			this.QueueFree();
			return true;
		}
		else
		{
			GD.Print("Can not add item to player inventory");
			return false;
		}
	}
	

}
