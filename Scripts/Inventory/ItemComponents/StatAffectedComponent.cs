using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class StatAffectedComponent : ItemComponent
{
	[Export] private Dictionary<Enums.StatType, double> statValues = new Dictionary<Enums.StatType, double>();
	public override void Initialize()
	{
		
	}


	public void AffectStats(Player player)
	{
		if (statValues == null || statValues.Count == 0) return;
		if(player == null) return;
		
		
		if(!player.TryGetPlayerComponent<PlayerStatHolder>(out var playerStatHolder)) return;
		
		
		foreach (var statValue in statValues)
		{
			if(!playerStatHolder.stats.ContainsKey(statValue.Key)) continue;
			
			playerStatHolder.stats[statValue.Key].ChangeValue(statValue.Value);
		}
	}
}
