using Godot;
using System;
using Godot.Collections;

public partial class HotBarInventoryUI : UIWindow
{
	[Export]public Player player;
	[Export] public int itemSlotCount = 1;
	private ItemSlot[] itemSlots;
	
	[Export] private PackedScene itemSlotScene;
	[Export] private Control slotHolder;
	
	
	private ulong _lastScrollTime = 0;
	[Export] public ulong scrollCooldownMs = 100;
	
	public int selectedSlotIndex
	{
		get
		{
			if (player == null)
			{
				GD.Print("player is null");
				return -1;
			}

			if (!player.TryGetPlayerComponent<PlayerInventory>(out PlayerInventory playerInventory))
			{
				GD.Print("player inventory is null");
				return -1;
			}
			
			return playerInventory.selectedItemIndex;

		}
		set
		{
			if (!player.TryGetPlayerComponent<PlayerInventory>(out PlayerInventory playerInventory))
			{
				GD.Print("player inventory is null");
			}
			
			playerInventory.selectedItemIndex = value;
		}
	}

	private void PlayerInventoryOnInventoryChanged(Inventory inventory)
	{
		UpdateItemSlots(inventory);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			// Check if enough time has passed since the last scroll
			ulong currentTime = Time.GetTicksMsec();
        
			if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
			{
				if (currentTime - _lastScrollTime > scrollCooldownMs)
				{
					SetSelectedSlotIndex(GetNextSlotIndex(true));
					_lastScrollTime = currentTime;
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
			{
				if (currentTime - _lastScrollTime > scrollCooldownMs)
				{
					SetSelectedSlotIndex(GetNextSlotIndex(false));
					_lastScrollTime = currentTime;
				}
			}
		}

		// Number keys logic
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode >= Key.Key1 && keyEvent.Keycode <= Key.Key9)
			{
				int index = (int)keyEvent.Keycode - (int)Key.Key1;
				if (index < itemSlotCount)
				{
					SetSelectedSlotIndex(index);
				}
			}
		}
	}

	
	private void PrepareUI()
	{

		
		// Clear existing  children
		foreach (var child in slotHolder.GetChildren())
		{
			child.QueueFree();
		}

		itemSlots = new ItemSlot[itemSlotCount];
		
		for (int i = 0; i < itemSlotCount; i++)
		{
			ItemSlot slot = itemSlotScene.Instantiate<ItemSlot>();
			slotHolder.AddChild(slot);
			itemSlots[i] = slot;
			slot.SetCurrentItem(null);
            
			// Set initial visual state for selection
			slot.slotButton.Flat = (i == selectedSlotIndex);
		}
	}

	public override void Initilize(UIWindow parent)
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
				playerInventory.mainInventory.InventoryChanged += PlayerInventoryOnInventoryChanged;
			}
		}
		PrepareUI();
	}


	private int GetNextSlotIndex(bool wheelUp)
	{
		int currentIndex = selectedSlotIndex;
		int nextIndex = -1;
		if (wheelUp)
		{
			nextIndex = currentIndex + 1;

			if (nextIndex >= itemSlotCount)
			{
				nextIndex = 0;
			}
		}
		else 
		{
			nextIndex = currentIndex - 1;

			if (nextIndex < 0)
			{
				nextIndex = itemSlotCount - 1;
			}
		}
		
		return nextIndex;
	}
	
	

	public void SetSelectedSlotIndex(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= itemSlots.Length) return;
		
		itemSlots[selectedSlotIndex].SetSelected(false);
		
		selectedSlotIndex = slotIndex;
		
		itemSlots[selectedSlotIndex].SetSelected(true);
	}

	public void UpdateItemSlots(Inventory inventory)
	{
		// Reset all slots to null first to handle removed items
		for (int i = 0; i < itemSlots.Length; i++)
		{
			itemSlots[i].SetCurrentItem(null);
		}

		int index = 0;
		foreach (var inventoryItem in inventory.Items)
		{
			if (index >= itemSlots.Length) break;
			
			if (InventoryManager.Instance.TryGetItemByName(inventoryItem.Key, out var itemData))
			{
				itemSlots[index].SetCurrentItem(itemData);
			}
			else
			{
				itemSlots[index].SetCurrentItem(null);
			}
			
			index++;
		}
	}


	public void UpdateorCreateItemSlot(ItemData item, int slotIndex)
	{
		ItemSlot slot = itemSlotScene.Instantiate() as ItemSlot;

		if (slot == null)
		{
			GD.Print("item slot instance null!");
			return;
		}
		itemSlots[slotIndex] = slot;
		slotHolder.AddChild(slot);
		slot.SetCurrentItem(item);
	}
}
