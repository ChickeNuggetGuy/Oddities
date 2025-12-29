using Godot;
using System;

[GlobalClass]
public partial class StatProgressBarUI : UIElement
{
	[Export] private Player player;
	[Export] private Enums.StatType _statType;
	[Export] public ProgressBar progressBar;


	protected override void Initilize(UIWindow parent)
	{
		
		base.Initilize(parent);
		if (!player.TryGetPlayerComponent<PlayerStatHolder>(out PlayerStatHolder statHolder))
		{
			GD.Print("PlayerStatHolder not found");
			return;
		}

		if (!statHolder.stats.TryGetValue(_statType, out var stat))
		{
			GD.Print("Stat not found");
			return;
		}

		if (stat == null)
		{
			GD.Print("Stat is null");
			return;
		}
		
		stat.ValueChanged += StatOnValueChanged;
		UpdateStatUI(stat.currentValue, stat.range);
	}

	private void StatOnValueChanged(double currentValue, Vector2I range)
	{
		UpdateStatUI(currentValue, range);
	}


	public void UpdateStatUI(double currentValue, Vector2I range)
	{
		progressBar.Value = currentValue;
		progressBar.MinValue = range.X;
		progressBar.MaxValue = range.Y;
	}
}
