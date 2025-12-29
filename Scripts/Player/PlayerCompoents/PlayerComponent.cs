using Godot;
using System;

public abstract partial class PlayerComponent : Node3D
{
	[Export] public Player parentPlayer;
	[Export] public bool updateOnDayChanged = false;
	
	public override void _Ready()
	{
		base._Ready();

		if (parentPlayer == null)
		{
			var parent = GetParent();

			if (parent != null && parent is Player)
			{
				parentPlayer = (Player)parent;
			}
		}
	}

	public void IntizializeCall(Player parent)
	{
		parentPlayer = parent;
		Initialize(parent);
		if (updateOnDayChanged)
		{
			DayManager.Instance.DayChanged += DayManagerOnDayChanged;
		}
	}

	protected virtual void DayManagerOnDayChanged(int currentDay)
	{
		return;
	}


	protected virtual void Initialize(Player parent)
	{
	
	}
	
}
