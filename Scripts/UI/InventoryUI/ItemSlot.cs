using Godot;
using System;

[GlobalClass]
public partial class ItemSlot : UIElement
{
	public ItemData CurrentItem { get; protected set; }
	[Export] public Button slotButton;
	[Export] public TextureRect itemIcon;
	[Export] public TextureRect borderImage;
	public bool isSelected {get; protected set;} = false;
	
	public void SetCurrentItem(ItemData item)
	{
		CurrentItem = item;
		
		if (CurrentItem != null)
			itemIcon.Texture = item.ItemIcon;
		else
			itemIcon.Texture = null;

	}
	
	public void SetSelected(bool selected)
	{
		isSelected = selected;
		if (isSelected)
		{
			borderImage.Show();
		}
		else
		{
			borderImage.Hide();
		}
	}
}
