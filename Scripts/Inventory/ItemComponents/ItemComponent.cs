using Godot;
using System;

[GlobalClass]
public abstract partial class ItemComponent : Resource
{
	public WorldItem ParentWorldItem {get;protected set;}


	public void InitializeCall(WorldItem parent)
	{
		ParentWorldItem = parent;
		Initialize();
	}
	
	public abstract void Initialize();

	
	public void TerminateCall(WorldItem parent)
	{
		Terminate();
	}
	public abstract void Terminate();
}
