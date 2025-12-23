using Godot;

[GlobalClass]
public partial class PlantableComponent : ItemComponent
{
	[Export] private Godot.Collections.Array<GrowthStage> stages = new();
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

	public ItemData GetHarvestResult()
	{
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