using Godot;
using System;

[GlobalClass]
public partial class Door : Node3D, IInteractable
{
	[Export] public Key InteractionKey { get; set; }
	[Export] public float openAngleDegrees = 90f;
	[Export] private bool animate = true;
	[Export] private float animationDuration = 0.5f;
	[Export] private Node3D pivot;

	public bool isOpen { get; set; } = false;
	private bool _isBusy = false;

	public void Interact(Player player, WorldItem parent)
	{
		if (!_isBusy) Toggle(player);
	}

	public void Toggle(Player player)
	{
		isOpen = !isOpen;
		float targetRad = isOpen ? Mathf.DegToRad(openAngleDegrees) : 0f;

		if (animate)
		{
			_isBusy = true;
			var tween = GetTree().CreateTween();
			tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
			tween.TweenProperty(pivot, "rotation:y", targetRad, animationDuration);
			tween.Finished += () => _isBusy = false;
		}
		else
		{
			Vector3 rot = pivot.Rotation;
			rot.Y = targetRad;
			pivot.Rotation = rot;
		}
	}

	public void Open(Player player)
	{
		if (!isOpen && !_isBusy) Toggle(player);
	}

	public void Close(Player player)
	{
		if (isOpen && !_isBusy) Toggle(player);
	}
}