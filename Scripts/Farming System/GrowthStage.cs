using Godot;

[GlobalClass]
public partial class GrowthStage : Resource
{
	[Export] public Mesh StageMesh;
	[Export] public int DaysToGrow;
	// If null, it's not harvestable yet. If set, this is what the player gets.
	[Export] public ItemData HarvestResult; 
}