using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Player : CharacterBody3D
{
	public Array<AllowedArea> CurrentAreas {get; protected set;} 
	[Export] private int playerID = -1;
	public bool IsSetup {get; private set;}

	public void Setup(int id)
	{
		playerID = id;
		CurrentAreas = new Array<AllowedArea>();
		InitializePlayerComponents();
		PlayerManager.Instance.TryAddplayer(playerID, this);
		IsSetup = true;
	}

	public bool TryGetPlayerComponent<T>(out T component) where T : Node
	{
		foreach (var child in GetChildren())
		{
			if (child is T comp)
			{
				component = comp;
				return true;
			}
		}
		component = null;
		return false;
	}

	public void InitializePlayerComponents()
	{
		foreach (var child in GetChildren())
		{
			if (child is PlayerComponent comp)
			{
				comp.IntizializeCall(this);
			}
		}
	}
}