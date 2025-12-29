using Godot;
using System;

[GlobalClass]
public partial class InputManager : Manager<InputManager>
{
	public bool InputBlocked {  get; set; } = false;
	
	protected override void Setup()
	{
		
	}

	public void ChangeMouseMode(Input.MouseModeEnum mode)
	{
		Input.MouseMode = mode;
	}
}
