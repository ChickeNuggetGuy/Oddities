using Godot;
using System;

public abstract partial class ManagerBase : Node
{



	public void SetupCall()
	{
		Setup();
	}


	protected abstract void Setup();
}
