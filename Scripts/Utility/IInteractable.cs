using Godot;
using System;

public interface IInteractable
{
	[Export] public Key InteracionKey { get; set; }
	void Interact(Player  player, WorldItem parent);
}
