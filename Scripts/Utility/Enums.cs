using Godot;
using System;

public partial class Enums 
{
	public enum InteractionBehavior
	{
		PICKUP,
		USE,
		DELETE,
		HOLD
	}
	
	public enum StatType
	{
		HEALTH,
		STAMINA,
		MANA
	}


	public enum UIType
	{
		NONE,
		ROOT,
		INTERACTUI,
		INVENTORYUI,
		STATSUI
	}

	public enum Season
	{
		SPRING,
		SUMMER,
		FALL,
		WINTER
	}
	
}
