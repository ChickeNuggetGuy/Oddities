public class InventorySlot
{
	public ItemData Item;
	public int Count;

	public bool IsEmpty => Item == null;
	public bool IsFull => Item != null && Count >= Item.maxStackSize;

	public void Clear()
	{
		Item = null;
		Count = 0;
	}
}