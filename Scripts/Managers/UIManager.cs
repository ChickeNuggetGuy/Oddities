using Godot;
using System;
using Godot.Collections;
[GlobalClass]
public partial class UIManager : Manager<UIManager>
{

	[Export] public Dictionary<Enums.UIType,UIWindow> uiWindows = new Dictionary<Enums.UIType,UIWindow>();
	[Export] private UIWindow rootWindow;
	protected override void Setup()
	{
		if (rootWindow == null)
		{
			GD.Print("Root Window is null");
			return;
		}
		
		rootWindow.InitilizeCall(null);
	}
}