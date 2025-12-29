using Godot;
using System;

[GlobalClass]
public partial class InventoryUI : UIWindow
{
	[Export] public Enums.InventoryType InventoryType;
	[Export]public Player player;
	[Export] public int itemSlotCount = 1;
	protected ItemSlot[] itemSlots;
	
	[Export] protected PackedScene itemSlotScene;
	[Export] protected Control slotHolder;
	
	public Inventory linkedInventory {get; protected set;}
	
	
	private void PlayerInventoryOnInventoryChanged(Inventory inventory)
	{
		UpdateItemSlots(inventory);
	}

	
	public void UpdateItemSlots(Inventory inventory)
	{
		for (int i = 0; i < itemSlots.Length; i++)
		{
			if (i < inventory.Slots.Length)
			{
				InventorySlot slotData = inventory.Slots[i];
				// Just pass the item and count directly from the slot
				itemSlots[i].SetCurrentItem(slotData.Item, slotData.Count);
			}
			else
			{
				itemSlots[i].SetCurrentItem(null, 0);
			}
		}
	}
	
	
	protected virtual void PrepareUI()
	{
		if(player == null || !player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory)) return;
		
		itemSlotCount = linkedInventory._maxItemSlots;
		// Clear existing  children
		foreach (var child in slotHolder.GetChildren())
		{
			child.QueueFree();
		}

		itemSlots = new ItemSlot[itemSlotCount];
		
		for (int i = 0; i < itemSlotCount; i++)
		{
			ItemSlot slot = itemSlotScene.Instantiate<ItemSlot>();
			slot.InitilizeCall(this);
			slotHolder.AddChild(slot);
			slot.Name = "Slot" + i;
			itemSlots[i] = slot;
			slot.SetCurrentItem(null, 0);
		}
	}

	protected override void Initilize(UIWindow parent)
	{
		base.Initilize(parent);
		
		if (player != null)
		{
			if (!player.TryGetPlayerComponent<PlayerInventory>(out var playerInventory))
			{
				return;
			}
			else
			{
				if(playerInventory.inventories.ContainsKey(InventoryType))
				{
					linkedInventory = playerInventory.inventories[InventoryType];
					linkedInventory.InventoryChanged += PlayerInventoryOnInventoryChanged;
				}
			}
		}
		PrepareUI();
	}
	
	
	public void UpdateorCreateItemSlot(ItemData item, int count, int slotIndex)
	{
		ItemSlot slot = itemSlotScene.Instantiate() as ItemSlot;

		if (slot == null)
		{
			GD.Print("item slot instance null!");
			return;
		}
		itemSlots[slotIndex] = slot;
		slotHolder.AddChild(slot);
		slot.SetCurrentItem(item, count);
	}


	public void ItemSlotPressed(ItemSlot slot)
	{
		GD.Print("Item slot pressed!");
		MouseHeldItemUI mouseUI = MouseHeldItemUI.Instance;
		if (mouseUI == null || linkedInventory == null || mouseUI.linkedInventory == null)
		{
			GD.Print("UI References missing!");
			return;
		}

		// Identify which index was clicked
		int slotIndex = Array.IndexOf(itemSlots, slot);
		if (slotIndex == -1) return;

		Inventory mouseInv = mouseUI.linkedInventory;
		InventorySlot clickedSlot = linkedInventory.GetSlot(slotIndex);


		InventorySlot mouseSlot = mouseInv.GetSlot(0);

		// Mouse is holding an item
		if (!mouseSlot.IsEmpty)
		{
			ItemData heldItem = mouseSlot.Item;

			if (clickedSlot.IsEmpty)
			{
				// Place item into empty slot
				Inventory.TryTransferItem(mouseInv, linkedInventory, heldItem, mouseSlot.Count, slotIndex);
			}
			else if (clickedSlot.Item == heldItem)
			{
				// Same item: Try to stack them
				Inventory.TryTransferItem(mouseInv, linkedInventory, heldItem, mouseSlot.Count, slotIndex);
			}
			else
			{
				// Different items: Swap them
				SwapSlotWithMouse(slotIndex, mouseInv, 0);
				mouseUI.ShowCall();
			}
		}
		// CASE 2: Mouse is empty, Slot has an item
		else if (!clickedSlot.IsEmpty)
		{
			// Pick up the item from this specific slot
			ItemData itemToPickUp = clickedSlot.Item;
			int amountToPickUp = clickedSlot.Count;


			if (mouseInv.CanAddItem(itemToPickUp, amountToPickUp))
			{
				linkedInventory.TryRemoveFromSlot(slotIndex, amountToPickUp);
				mouseInv.TryAddItem(itemToPickUp, amountToPickUp);
				mouseUI.ShowCall();
			}
		}
	}

	private void SwapSlotWithMouse(int targetIndex, Inventory mouseInv, int mouseIndex)
	{
		InventorySlot invSlot = linkedInventory.GetSlot(targetIndex);
		InventorySlot mSlot = mouseInv.GetSlot(mouseIndex);
		
		ItemData tempItem = invSlot.Item;
		int tempCount = invSlot.Count;

		// Move mouse to inventory
		invSlot.Item = mSlot.Item;
		invSlot.Count = mSlot.Count;

		// Move temp (old inventory) to mouse
		mSlot.Item = tempItem;
		mSlot.Count = tempCount;
		
	}
}
