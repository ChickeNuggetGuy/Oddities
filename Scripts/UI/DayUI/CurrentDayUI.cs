using Godot;
using System;

public partial class CurrentDayUI : UIWindow
{
	[Export] private Label currentDayLabel;
	[Export] private TextureProgressBar dayProgressBar;


	protected override void Initilize(UIWindow parent)
	{
		base.Initilize(parent);
		DayManager.Instance.TimeChanged += DayManagerOnTimeChanged;
		
		
		
		UpdateUI(DayManager.Instance.CurrentDay, DayManager.Instance.CurrentHour, DayManager.Instance.CurrentMinutes);
	}

	private void DayManagerOnTimeChanged(int currentDay, int hours, int minutes)
	{
		UpdateUI(currentDay, hours, minutes);
	}


	private void UpdateUI(int currentDay = -1, int hour = -1, int minute = -1)
	{
		String newTimeText = $"Current Day: ";
		currentDayLabel.Text =newTimeText + DayManager.Instance.GetFormattedTime();

	}
}
