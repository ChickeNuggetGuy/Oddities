using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PlantableComponent : ItemComponent
{
	[Export] private Array<GrowthStage> stages = new();
	[Export] public Dictionary<Enums.StatType, double> plantingCost = new Dictionary<Enums.StatType, double>();
	protected int currentStageIndex = 0;
	protected int daysGrown = 0;

	public bool IsMature => currentStageIndex >= stages.Count - 1;

	public override void Initialize()
	{
		DayManager.Instance.DayChanged += OnDayChanged;
	}

	public override void Terminate()
	{
		DayManager.Instance.DayChanged -= OnDayChanged;
	}

	private void OnDayChanged(int currentDay)
	{
		daysGrown++;

		if (currentStageIndex < stages.Count - 1)
		{
			if (daysGrown >= stages[currentStageIndex].DaysToGrow)
			{
				currentStageIndex++;
				UpdateVisuals();
				
			}

		}
	}

	private void UpdateVisuals()
	{
		if (ParentWorldItem?.itemMesh != null)
		{
			ParentWorldItem.itemMesh.Mesh = stages[currentStageIndex].StageMesh;
			daysGrown = 0;
		}
	}

	public Dictionary<ItemData, Variant> GetHarvestResult(out GrowthStage stage)
	{
		stage = stages[currentStageIndex];
		if (IsMature)
		{
			return stages[currentStageIndex].HarvestResult;
		}
		return null;
	}
	
	public GrowthStage GetGrowthStage(int index)
	{
		GrowthStage stage = stages[index];
		return stage;
	}
}