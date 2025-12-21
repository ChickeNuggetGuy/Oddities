using Godot;
using System;

[GlobalClass]
public partial class DayManager : Manager<DayManager>
{
    [ExportGroup("Components")]
    [Export] public DirectionalLight3D SunLight;
    [Export] public UIWindow EndDayWindow;

    [ExportGroup("Time Settings")]
    [Export] public int HoursInDay = 24;
    [Export] public int MinuteIncrements = 10;
    [Export] public float SecondsPerIncrement = 1.0f;

    public int CurrentDay { get; protected set; } = 1;
    public int CurrentHour { get; private set; } = 6;
    public int CurrentMinutes { get; private set; } = 0;
    public Enums.Season CurrentSeason { get; protected set; } = Enums.Season.SPRING;

    private float _timeAccumulator = 0.0f;

    #region Signals
    [Signal] public delegate void DayChangedEventHandler(int currentDay);
    [Signal] public delegate void TimeChangedEventHandler(int currentDay, int hours, int minutes);
    [Signal] public delegate void SeasonChangedEventHandler(int newSeason);
    #endregion

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateTime((float)delta);
        UpdateSunPosition();
    }

    private void UpdateTime(float delta)
    {
        _timeAccumulator += delta;

        if (_timeAccumulator >= SecondsPerIncrement)
        {
            _timeAccumulator = 0;
            AdvanceMinutes(MinuteIncrements);
        }
    }

    private void UpdateSunPosition()
    {
        if (SunLight == null) return;

        // Calculate total minutes passed in the current day
        float totalMinutesInDay = HoursInDay * 60;
        float currentTotalMinutes = (CurrentHour * 60) + CurrentMinutes;
        
        // Add the progress toward the next increment for smooth movement
        float subMinuteProgress = (_timeAccumulator / SecondsPerIncrement) * MinuteIncrements;
        float preciseMinutes = currentTotalMinutes + subMinuteProgress;

        // Get 0.0 to 1.0 progress of the day
        float dayProgress = preciseMinutes / totalMinutesInDay;

        // Rotation Logic:
        // -90 degrees is straight up (Noon). 
        // We want to rotate 360 degrees over the course of the day.
        // We offset by 90 or 180 depending on where you want the day to start.
        float rotationAngle = Mathf.Lerp(0.0f, 360.0f, dayProgress);
        
        // Apply rotation (Converting degrees to radians)
        Vector3 newRotation = SunLight.RotationDegrees;
        newRotation.X = rotationAngle - 90.0f; // Offset so 12:00 is overhead
        SunLight.RotationDegrees = newRotation;

        // Bonus: Adjust light energy so it's dark at night
        float nightMultiplier = Mathf.Sin(Mathf.DegToRad(rotationAngle));
        SunLight.LightEnergy = Mathf.Max(0, nightMultiplier * 1.5f);
    }

    private void AdvanceMinutes(int minutes)
    {
        CurrentMinutes += minutes;
        if (CurrentMinutes >= 60)
        {
            CurrentMinutes = 0;
            AdvanceHour();
        }
        EmitSignal(SignalName.TimeChanged, CurrentDay, CurrentHour, CurrentMinutes);
    }

    private void AdvanceHour()
    {
        CurrentHour++;
        if (CurrentHour >= HoursInDay)
        {
            CurrentHour = 0;
            TryAdvanceDay();
        }
    }

    public void TryAdvanceDay()
    {
        CurrentDay++;
        CurrentHour = 6;
        CurrentMinutes = 0;
        if (CurrentDay > 30)
        {
            CurrentDay = 1;
            AdvanceSeason();
        }
        EmitSignal(SignalName.DayChanged, CurrentDay);
        EmitSignal(SignalName.TimeChanged, CurrentDay, CurrentHour, CurrentMinutes);
    }

    private void AdvanceSeason()
    {
        int nextSeason = ((int)CurrentSeason + 1) % 4;
        CurrentSeason = (Enums.Season)nextSeason;
        EmitSignal(SignalName.SeasonChanged, (int)CurrentSeason);
        EmitSignal(SignalName.TimeChanged, CurrentDay, CurrentHour, CurrentMinutes);
    }

    protected override void Setup() { }
}