using Godot;
using System;

public partial class Enums 
{
	public enum InteractionBehavior
	{
		PICKUP,
		USE,
		DELETE,
		HOLD,
		HARVEST
	}
	
	public enum StatType
	{
		HEALTH,
		STAMINA,
		MANA
	}


	public enum StatDayBehavior
	{
		NONE,
		INCREMENT,
		DECREMENT,
		MIN,
		MAX
	}

	public enum UIType
	{
		NONE,
		ROOT,
		INTERACTUI,
		INVENTORYUI,
		HOTBARUI,
		STATSUI,
		MOUSEHELDUI
	}

	public enum Season
	{
		SPRING,
		SUMMER,
		FALL,
		WINTER
	}


	public enum InventoryType
	{
		NONE,
		HOTBAR,
		MAIN,
		MOUSEHELD,
	}
}
