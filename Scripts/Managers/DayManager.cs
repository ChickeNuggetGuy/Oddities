using Godot;
using System;

[GlobalClass]
public partial class DayManager : Manager<DayManager>
{
    [ExportGroup("Components")]
    [Export] public DirectionalLight3D SunLight;
    [Export] public WorldEnvironment WorldEnv; // Optional: To control ambient light
    [Export] public UIWindow EndDayWindow;

    [ExportGroup("Time Config")]
    [Export] public int HoursInDay = 24;
    [Export] public int DaysPerSeason = 30;
    [Export] public int MorningHour = 6;
    
    [ExportGroup("Simulation Speed")]
    [Export] public int MinuteIncrements = 10;
    [Export] public float SecondsPerIncrement = 1.0f;
    [Export] public bool IsTimePaused = false;

    [ExportGroup("Visuals")]
    [Export] public Gradient SunColorGradient;
    [Export] public Curve LightEnergyCurve; 
    [Export] public float MaxSunEnergy = 1.5f;
    [Export(PropertyHint.Range, "-90, 90")] public float SunLatitude = -15.0f;

    public int CurrentDay { get; private set; } = 1;
    public int CurrentHour { get; private set; } = 6;
    public int CurrentMinutes { get; private set; } = 0;
    public Enums.Season CurrentSeason { get; private set; } = Enums.Season.SPRING;

    private float _timeAccumulator = 0.0f;
    private double _totalSecondsPlayed = 0;

    #region Signals
    [Signal] public delegate void DayChangedEventHandler(int currentDay);
    [Signal] public delegate void TimeChangedEventHandler(int currentDay, int hours, int minutes);
    [Signal] public delegate void SeasonChangedEventHandler(int newSeason);
    #endregion

    
        
    public override void _Process(double delta)
    {
        base._Process(delta);

        // Update visuals every frame for smooth shadows/sun movement
        UpdateSunPosition();

        if (!IsTimePaused)
        {
            ProcessTimeLogic((float)delta);
        }
    }

    private void ProcessTimeLogic(float delta)
    {
        _timeAccumulator += delta;
        _totalSecondsPlayed += delta;

        if (_timeAccumulator >= SecondsPerIncrement)
        {
            // Calculate how many increments passed (in case of lag spike)
            int steps = Mathf.FloorToInt(_timeAccumulator / SecondsPerIncrement);
            _timeAccumulator -= steps * SecondsPerIncrement;
            
            AdvanceMinutes(steps * MinuteIncrements);
        }
    }

   private void UpdateSunPosition()
{
    if (SunLight == null) return;

    // 1. Calculate Progress (0.0 = Midnight, 0.5 = Noon, 1.0 = Midnight)
    float totalMinutesInDay = HoursInDay * 60.0f;
    float currentTotalMinutes = (CurrentHour * 60) + CurrentMinutes;
    float subMinuteProgress = (_timeAccumulator / SecondsPerIncrement) * MinuteIncrements;
    float preciseDayProgress = (currentTotalMinutes + subMinuteProgress) / totalMinutesInDay;

    // 2. FIXED ROTATION LOGIC
    // Midnight (0.0) -> 90 degrees (Pointing UP)
    // Noon (0.5)     -> 270 degrees / -90 degrees (Pointing DOWN)
    // Midnight (1.0) -> 450 degrees / 90 degrees (Pointing UP)
    float rotationX = (preciseDayProgress * 360.0f) + 90.0f;
    SunLight.RotationDegrees = new Vector3(rotationX, SunLatitude, 0);

    // 3. Apply Visuals
    if (LightEnergyCurve != null)
    {
        // Use the progress to sample the curve
        SunLight.LightEnergy = LightEnergyCurve.Sample(preciseDayProgress) * MaxSunEnergy;
    }

    if (SunColorGradient != null)
    {
        SunLight.LightColor = SunColorGradient.Sample(preciseDayProgress);
    }
}

protected override void Setup()
{
    
    if (SunColorGradient == null)
    {
        SunColorGradient = new Gradient();
        
        // Define specific colors
        Color nightColor = new Color("#0d1229");
        Color dawnColor = new Color("#ff7b47");
        Color dayColor = new Color("#ffffff");
        Color duskColor = new Color("#d961a8");

        // Map colors to time of day
        SunColorGradient.SetOffset(0, 0.0f);
        SunColorGradient.SetColor(0, nightColor);

        SunColorGradient.AddPoint(0.20f, nightColor);
        SunColorGradient.AddPoint(0.25f, dawnColor);
        SunColorGradient.AddPoint(0.35f, dayColor);
        SunColorGradient.AddPoint(0.65f, dayColor);   
        SunColorGradient.AddPoint(0.75f, duskColor); 
        SunColorGradient.AddPoint(0.85f, nightColor); 
    }

    if (LightEnergyCurve == null)
    {
        LightEnergyCurve = new Curve();
        // Clear default points if any
        while(LightEnergyCurve.GetPointCount() > 0) LightEnergyCurve.RemovePoint(0);

        // Define energy (Brightness)
        LightEnergyCurve.AddPoint(new Vector2(0.0f, 0.0f));
        LightEnergyCurve.AddPoint(new Vector2(0.20f, 0.0f));
        LightEnergyCurve.AddPoint(new Vector2(0.30f, 1.0f));
        LightEnergyCurve.AddPoint(new Vector2(0.70f, 1.0f));
        LightEnergyCurve.AddPoint(new Vector2(0.80f, 0.0f));
        LightEnergyCurve.AddPoint(new Vector2(1.0f, 0.0f));
        

        for(int i = 0; i < LightEnergyCurve.PointCount; i++) {
            LightEnergyCurve.SetPointLeftMode(i, Curve.TangentMode.Linear);
            LightEnergyCurve.SetPointRightMode(i, Curve.TangentMode.Linear);
        }
    }
}

    private void AdvanceMinutes(int minutesToAdd)
    {
        CurrentMinutes += minutesToAdd;
        
        while (CurrentMinutes >= 60)
        {
            CurrentMinutes -= 60;
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
            AdvanceDayNatural(); 
        }
    }

    private void AdvanceDayNatural()
    {
        CurrentDay++;
        CheckSeasonChange();
        EmitSignal(SignalName.DayChanged, CurrentDay);
    }

    /// <summary>
    /// Call this when the player sleeps to skip to the next morning.
    /// </summary>
    public void StartNextDay()
    {
        CurrentDay++;
        CurrentHour = MorningHour;
        CurrentMinutes = 0;
        _timeAccumulator = 0;

        CheckSeasonChange();
        
        EmitSignal(SignalName.DayChanged, CurrentDay);
        EmitSignal(SignalName.TimeChanged, CurrentDay, CurrentHour, CurrentMinutes);
        
        // Update sun immediately so there isn't a visual frame flicker
        UpdateSunPosition(); 
    }

    private void CheckSeasonChange()
    {
        if (CurrentDay > DaysPerSeason)
        {
            CurrentDay = 1;
            int nextSeason = ((int)CurrentSeason + 1) % 4;
            CurrentSeason = (Enums.Season)nextSeason;
            EmitSignal(SignalName.SeasonChanged, (int)CurrentSeason);
        }
    }

    /// <summary>
    /// Returns time formatted as "06:30 AM" or "14:30" based on preference.
    /// </summary>
    public string GetFormattedTime(bool use24Hour = false)
    {
        if (use24Hour)
        {
            return $"{CurrentHour:D2}:{CurrentMinutes:D2}";
        }
        else
        {
            string suffix = CurrentHour >= 12 ? "PM" : "AM";
            int hourDisplay = CurrentHour % 12;
            if (hourDisplay == 0) hourDisplay = 12;
            return $"{hourDisplay:D2}:{CurrentMinutes:D2} {suffix}";
        }
    }
}