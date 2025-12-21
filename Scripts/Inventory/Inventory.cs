using Godot;
using System;
using System.Linq;
using Godot.Collections;

[GlobalClass]
public partial class Inventory : Resource
{
	[Signal] public delegate void InventoryChangedEventHandler(Inventory inventory);

	[Export] private int _maxItemSlots = 10;
	
	public Dictionary<string, int> Items = new();


	public bool CanAddItem(ItemData itemData, int count = 1)
	{
		if (itemData == null) return false;

		bool hasItem = Items.ContainsKey(itemData.ItemName);
        
		// 1. If we DON'T have the item, check if we have a free slot
		if (!hasItem)
		{
			if (_maxItemSlots != -1 && Items.Count >= _maxItemSlots)
			{
				GD.Print("Inventory Full: No more unique slots available.");
				return false;
			}
		}
		else 
		{
			// 2. If we DO have the item, check if adding would exceed its max stack size
			if (Items[itemData.ItemName] + count > itemData.maxStackSize)
			{
				GD.Print($"Inventory Full: {itemData.ItemName} stack is full.");
				return false;
			}
		}

		return true; 
	}

	public bool TryAddItem(ItemData itemData, int count = 1)
	{
		if (!CanAddItem(itemData, count)) return false;

		if (Items.ContainsKey(itemData.ItemName))
		{
			Items[itemData.ItemName] += count;
		}
		else
		{
			Items.Add(itemData.ItemName, count);
		}

		EmitSignal(SignalName.InventoryChanged, this);
		GD.Print($"Added {count} to inventory. There are {Items[itemData.ItemName]} of this item");
		return true;
	}

	public bool TryRemoveItem(ItemData itemData, int count = 1)
	{
		if (!Items.ContainsKey(itemData.ItemName)) return false;

		if (Items[itemData.ItemName] > count)
		{
			Items[itemData.ItemName] -= count;
		}
		else
		{
			// If we remove all, clean up the dictionary key
			Items.Remove(itemData.ItemName);
		}

		GD.Print("Item removed");
		EmitSignal(SignalName.InventoryChanged, this);
		return true;
	}


	public bool TryGetItemAtIndex(int index, out ItemData itemData)
	{
		itemData = null; 
		if(Items == null || Items.Count <= index) return false;

		if (InventoryManager.Instance.TryGetItemByName(Items.Keys.ToList()[index], out itemData))
		{
			return true;
		}
		
		return itemData != null;
	}
}