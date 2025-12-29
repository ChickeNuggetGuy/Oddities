using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerStatHolder : PlayerComponent
{
	[Export] public Godot.Collections.Dictionary<Enums.StatType, Stat> stats;



	protected override void DayManagerOnDayChanged(int currentDay)
	{
		foreach (Stat stat in stats.Values)
		{
			stat.DayChanged();
		}
	}


	public bool CanAffordStatCost(Godot.Collections.Dictionary<Enums.StatType, double> statCosts)
	{
		bool canAfford = true;

		foreach (var statCost in statCosts)
		{
			if (stats.ContainsKey(statCost.Key))
			{
				if (statCost.Value > stats[statCost.Key].currentValue)
				{
					canAfford = false;
					break;
				}
			}
		}
		return canAfford;
	}
	
	
	public bool TrySpendStatCost(Godot.Collections.Dictionary<Enums.StatType, double> statCosts)
	{
		if (!CanAffordStatCost(statCosts))
		{
			return false;
		}
		
		foreach (var statCost in statCosts)
		{
			if (stats.ContainsKey(statCost.Key))
			{
				stats[statCost.Key].TryRemoveValue(statCost.Value);
			}
		}
		
		return true;
	}
}
