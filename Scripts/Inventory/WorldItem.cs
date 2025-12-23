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

	// Keep your Pickup/GiveItem logic as is, it handles the UI and signals perfectly
	public virtual void Pickup(Player player, ItemData dataToGive)
	{
		GiveItemToPlayer(player, dataToGive);
	}

	private void GiveItemToPlayer(Player player,ItemData data)
	{
		if (!player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory)) return;

		if (playerInventory.mainInventory.TryAddItem(data, stackSize))
		{
			GD.Print($"Harvested: {data.ItemName}");
			EmitSignal(SignalName.PickedUp, this, this.GlobalPosition);
			this.QueueFree();
		}
		else
		{
			GD.Print("Can not add item to player inventory");
		}
	}
	

}
