using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class WorldItem : RigidBody3D
{
	[Export] public ItemData itemData;
	[Export] public int stackSize = 1;


	public WorldItem()
	{
		itemData = null;
	}
	
	public WorldItem(ItemData data, int stack = 1)
	{
		itemData = data;
		stackSize = stack;
	}

	public override void _Ready()
	{
		base._Ready();

		foreach (ItemComponent component in itemData.components)
		{
			component.InitializeCall(this);
		}
	}

	public bool TryGetItemComponent<T>(out T component) where T : ItemComponent
	{
		foreach (ItemComponent itemComponent in itemData.components)
		{
			if (itemComponent is T)
			{
				component = itemComponent as T;
				return true;
			}
		}
		component = null;
		return false;
	}
}
