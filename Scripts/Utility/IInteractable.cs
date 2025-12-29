using Godot;
using System;

public interface IInteractable
{
	[Export] public Key InteractionKey { get; set; }
	void Interact(Player  player, WorldItem parent);
}
