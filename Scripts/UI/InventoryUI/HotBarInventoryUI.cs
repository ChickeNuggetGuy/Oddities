using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class HotBarInventoryUI : InventoryUI
{
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

	protected override void PrepareUI()
	{
		base.PrepareUI();
		
		
		for (int i = 0; i < itemSlotCount; i++)
		{
			ItemSlot slot = itemSlots[i];
			
			slot.slotButton.Flat = (i == selectedSlotIndex);
		}
	}
}
