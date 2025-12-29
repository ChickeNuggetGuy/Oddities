using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GrowthStage : Resource
{
	[Export] public Mesh StageMesh;
	[Export] public int DaysToGrow;
	[Export] public Dictionary<ItemData, Variant> HarvestResult;
	[Export] public Dictionary<Enums.StatType, double> harvestCost =  new Dictionary<Enums.StatType, double>();
}