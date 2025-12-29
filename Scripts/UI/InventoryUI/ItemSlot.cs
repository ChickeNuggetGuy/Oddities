using Godot;
using System;

[GlobalClass]
public partial class ItemSlot : UIElement
{
	public InventoryUI parentInventoryUI;
	public ItemData CurrentItem { get; protected set; }
	[Export] public Button slotButton;
	public int itemCount = 0;
	[Export] public TextureRect borderImage;
	[Export] public Label countLabel;
	public bool isSelected {get; protected set;} = false;
	
	public void SetCurrentItem(ItemData item, int count)
	{
		CurrentItem = item;
		itemCount = count;
		if (CurrentItem != null)
		{
			countLabel.Text = count.ToString();
			
			if(slotButton != null)
				slotButton.Icon = item.ItemIcon;
		}
		else
		{
			countLabel.Text = "";
			
			if(slotButton != null)
				slotButton.Icon  = null;
		}
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

	protected override void Initilize(UIWindow parent)
	{
		if (parent is InventoryUI)
		{
			parentInventoryUI = (InventoryUI)parent;
		}
		base.Initilize(parent);
		
		if(slotButton != null)
			slotButton.Pressed += SlotButtonOnPressed;
	}

	
	private void SlotButtonOnPressed()
	{
		GD.Print("Slot button pressed!");
		parentInventoryUI?.ItemSlotPressed(this);
	}

	public override void _ExitTree()
	{
		if(slotButton != null)
			slotButton.Pressed -= SlotButtonOnPressed;
		base._ExitTree();
		
	}
}
