using Godot;
using System;

[GlobalClass]
public abstract partial class ItemComponent : Resource
{
	public WorldItem ParentWorldItem {get;protected set;}


	public void InitializeCall(WorldItem parent)
	{
		ParentWorldItem = parent;
	}
	
	public abstract void Initialize();
}
