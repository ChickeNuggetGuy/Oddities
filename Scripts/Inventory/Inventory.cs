using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Inventory : Resource
{
    [Signal]
    public delegate void InventoryChangedEventHandler(Inventory inventory);

    [Export]
    public int _maxItemSlots;

    public InventorySlot[] Slots;

    public int ItemCount
    {
        get
        {
            if (Slots == null)
            {
                return 0;
            }

            int count = 0;
            foreach (var slot in Slots)
            {
                if (slot.Item != null)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public Inventory()
    {
        InitializeInventory();
    }

    public Inventory(int maxSlots)
    {
        _maxItemSlots = maxSlots;
        InitializeInventory();
    }

    public void InitializeInventory()
    {
        Slots = new InventorySlot[_maxItemSlots];
        for (int i = 0; i < _maxItemSlots; i++)
        {
            Slots[i] = new InventorySlot();
        }
    }

    public bool CanAddItem(ItemData itemData, int count = 1)
    {
        if (itemData == null || count <= 0)
        {
            return false;
        }
        if (Slots == null)
        {
            return false;
        }

        int remainingToFit = count;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item == itemData)
            {
                int spaceInStack = itemData.maxStackSize - Slots[i].Count;
                remainingToFit -= spaceInStack;
            }
            else if (Slots[i].IsEmpty)
            {
                remainingToFit -= itemData.maxStackSize;
            }

            if (remainingToFit <= 0)
            {
                return true;
            }
        }

        return false;
    }

    public int GetTotalAmount(ItemData itemData)
    {
        if (itemData == null || Slots == null)
        {
            return 0;
        }
        int total = 0;
        foreach (var slot in Slots)
        {
            if (slot.Item == itemData)
            {
                total += slot.Count;
            }
        }
        return total;
    }

    public bool TryAddItem(ItemData itemData, int count = 1, int slotIndex = -1)
    {
        if (!CanAddItem(itemData, count))
        {
            GD.Print("Inventory Full: Could not add item.");
            return false;
        }

        int remainingToAdd = count;

        //Try specific slot if provided
        if (slotIndex != -1)
        {
            InventorySlot targetSlot = GetSlot(slotIndex);
            if (
                targetSlot != null &&
                (targetSlot.IsEmpty || targetSlot.Item == itemData)
            )
            {
                int canTake = itemData.maxStackSize - targetSlot.Count;
                int amountToAdd = Math.Min(remainingToAdd, canTake);

                if (amountToAdd > 0)
                {
                    targetSlot.Item = itemData;
                    targetSlot.Count += amountToAdd;
                    remainingToAdd -= amountToAdd;
                }
            }
        }

        //Try to add to existing stacks first
        if (remainingToAdd > 0)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].Item == itemData && !Slots[i].IsFull)
                {
                    int canTake = itemData.maxStackSize - Slots[i].Count;
                    int amountToAdd = Math.Min(remainingToAdd, canTake);

                    Slots[i].Count += amountToAdd;
                    remainingToAdd -= amountToAdd;
                }
                if (remainingToAdd <= 0)
                {
                    break;
                }
            }
        }

        //Try to add to empty slots if there's still items left
        if (remainingToAdd > 0)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty)
                {
                    int amountToAdd = Math.Min(
                        remainingToAdd,
                        itemData.maxStackSize
                    );
                    Slots[i].Item = itemData;
                    Slots[i].Count = amountToAdd;
                    remainingToAdd -= amountToAdd;
                }
                if (remainingToAdd <= 0)
                {
                    break;
                }
            }
        }

        EmitSignal(SignalName.InventoryChanged, this);
        return true;
    }

    public bool TryRemoveItem(
        ItemData itemData,
        int count = 1,
        int slotIndex = -1
    )
    {
        // Check if we even have enough to remove first
        if (GetTotalAmount(itemData) < count)
        {
            return false;
        }

        int remainingToRemove = count;

        // 1. Try to remove from the specific slot first if provided
        if (slotIndex != -1)
        {
            InventorySlot targetSlot = GetSlot(slotIndex);
            if (targetSlot != null && targetSlot.Item == itemData)
            {
                int amountToRemove = Math.Min(remainingToRemove, targetSlot.Count);
                targetSlot.Count -= amountToRemove;
                remainingToRemove -= amountToRemove;

                if (targetSlot.Count <= 0)
                {
                    targetSlot.Clear();
                }
            }
        }

        // 2. Remove remaining items across multiple stacks (last in, first out)
        if (remainingToRemove > 0)
        {
            for (int i = Slots.Length - 1; i >= 0; i--)
            {
                if (Slots[i].Item == itemData)
                {
                    if (Slots[i].Count > remainingToRemove)
                    {
                        Slots[i].Count -= remainingToRemove;
                        remainingToRemove = 0;
                    }
                    else
                    {
                        remainingToRemove -= Slots[i].Count;
                        Slots[i].Clear();
                    }
                }
                if (remainingToRemove <= 0)
                {
                    break;
                }
            }
        }

        EmitSignal(SignalName.InventoryChanged, this);
        return true;
    }

    public InventorySlot GetSlot(int index)
    {
        if (Slots == null)
        {
            GD.Print("Slots not initialized.");
            return null;
        }

        if (index < 0)
        {
            GD.Print("Index out of range 0.");
            return null;
        }
        if (index >= Slots.Length)
        {
            GD.Print($"Index {index} out of range of {Slots.Length}.");
            return null;
        }
        return Slots[index];
    }

    public bool TryRemoveFromSlot(int index, int count = 1)
    {
        InventorySlot slot = GetSlot(index);
        if (slot == null || slot.IsEmpty || slot.Count < count)
        {
            return false;
        }

        slot.Count -= count;
        if (slot.Count <= 0)
        {
            slot.Clear();
        }

        EmitSignal(SignalName.InventoryChanged, this);
        return true;
    }

    public static bool TryTransferItem(
        Inventory fromInventory,
        Inventory toInventory,
        ItemData itemToTransfer,
        int amount,
        int slotIndex = -1
    ) {
        if (
            fromInventory == null ||
            toInventory == null ||
            itemToTransfer == null
        ) {
            return false;
        }

        // check source has enough items
        if (fromInventory.GetTotalAmount(itemToTransfer) < amount)
        {
            GD.Print("Transfer Failed: Source lacks sufficient items.");
            return false;
        }

        // check destination has enough space
        if (!toInventory.CanAddItem(itemToTransfer, amount))
        {
            GD.Print("Transfer Failed: Destination cannot fit items.");
            return false;
        }

        // Perform transfer
        fromInventory.TryRemoveItem(itemToTransfer, amount);
        toInventory.TryAddItem(itemToTransfer, amount, slotIndex != -1 ? slotIndex : -1);

        return true;
    }
}