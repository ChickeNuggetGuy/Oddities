using Godot;
using System;

public partial class UIElement : Control
{
	public UIWindow ParentWindow {get; protected set;}
	
	
	public void InitilizeCall(UIWindow parent)
	{
		ParentWindow = parent;
		Initilize(parent);
	}

	protected virtual void Initilize(UIWindow parent)
	{

	}
}
