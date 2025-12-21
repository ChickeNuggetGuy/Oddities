using Godot;
using System;

public abstract partial class Manager<T> : ManagerBase where T : ManagerBase
{
	public static T Instance { get; protected set; }
	
	public override void _EnterTree()
	{
		base._EnterTree();
		if (Instance != null)
		{
			if (Instance != this)
			{
				GD.PrintErr("Only one instance of UIManager allowed, destroying self");
				this.QueueFree();
			}
		}
		else
		{
			Instance = this as T;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Instance == this)
		{
			Instance = null;
		}
	}
	
}
