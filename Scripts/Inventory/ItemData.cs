using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class ItemData : Resource
{
	[Export] public string ItemName = "";
	[Export] public string ItemDescription = "";
	[Export] public Texture2D ItemIcon;
	[Export] public int itemID = 0;
	
	[Export] public int maxStackSize = 1;
	[Export(PropertyHint.ResourceType, "ItemComponent")] public Array<ItemComponent>  components = new Array<ItemComponent>();



	public bool TryGetItemComponent<T>(out T itemComponent) where T : ItemComponent
	{
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null && components[i] is T)
			{
				itemComponent = (T)components[i];
				return true;
			}
		}
		itemComponent = null;
		return false;
	}
}
