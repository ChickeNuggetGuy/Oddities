using Godot;
using System;

[GlobalClass]
public partial class MouseHeldItemUI : InventoryUI
{
	public static MouseHeldItemUI Instance;


	public override void _Ready()
	{
		base._Ready();
		InitilizeCall(UIManager.Instance.rootWindow);
	}

	protected override void Initilize(UIWindow parent)
	{
		Instance = this;
		linkedInventory = new Inventory(1);
		linkedInventory.InitializeInventory();
		PrepareUI();

		foreach (ItemSlot itemSlot in itemSlots)
		{
			itemSlot.MouseFilter = MouseFilterEnum.Ignore;
		}

	}
	
	

	public override void _Process(double delta)
	{
		if (!IsShown)
			return;
		
		base._Process(delta);

		this.GlobalPosition = GetGlobalMousePosition() + new Vector2(20, 5);//- (Size / 2) ;

	}
}
