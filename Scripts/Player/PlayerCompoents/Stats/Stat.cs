using Godot;
using System;

[GlobalClass]
public partial class Stat : Resource
{
	[Export] public Enums.StatType statType;
	[Export] public double currentValue;
	[Export] public Vector2I range;

	[Export] protected Enums.StatDayBehavior dayChangedBehavior = Enums.StatDayBehavior.NONE;

	[ExportGroup("Day Changes")] [Export] public double incrementAmount;
	[Export] public double incrementRate;
	
	[Export] public double decrementAmount;
	[Export] public double decrementRate;

	
	[ExportGroup("Signals")]
	[Export] public bool signalOnMin;
	[Export] public bool signalOnMax;

	
	
	#region Signals
	[Signal] public delegate void ValueChangedEventHandler(double currentValue, Vector2I range);
	[Signal] public delegate void MinValueReachedEventHandler(double currentValue, Vector2I range);
	[Signal] public delegate void MaxValueReachedEventHandler(double currentValue, Vector2I range);
	
	#endregion



	public bool TryAddValue(double amount)
	{
		if (currentValue + amount >= range.Y)
		{
			currentValue = range.Y;
			if (signalOnMax)
			{
				EmitSignal(SignalName.MaxValueReached, currentValue, range);
			}
		}
		else
		{
			currentValue += amount;
		}
		EmitSignal(SignalName.ValueChanged, currentValue, range);
		
		return true;
	}
	
	public bool TryRemoveValue(double amount)
	{
		if (currentValue - amount <= range.X)
		{
			currentValue = range.X;
			if (signalOnMin)
			{
				EmitSignal(SignalName.MinValueReached, currentValue, range);
			}
		}
		else
		{
			currentValue -= amount;
		}
		EmitSignal(SignalName.ValueChanged, currentValue, range);
		return true;
	}


	public void ChangeValue(double amount)
	{
		if (double.IsNegative(amount))
		{
			//Is Negative 
			TryRemoveValue(double.Abs(amount));
		}
		else
		{
			TryAddValue(double.Abs(amount));
		}
		
	}


	public void SetValue(double amount)
	{
		if (amount < range.X)
		{
			amount = range.X;
			EmitSignal(SignalName.MinValueReached, currentValue, range);
		}

		if (amount > range.Y)
		{
			amount = range.Y;
			EmitSignal(SignalName.MaxValueReached, currentValue, range);
		}
		
		currentValue = amount;
		EmitSignal(SignalName.ValueChanged, currentValue, range);
	}

	public void DayChanged()
	{
		switch (dayChangedBehavior)
		{
			case Enums.StatDayBehavior.NONE:
				break;
			case Enums.StatDayBehavior.DECREMENT:
				if (decrementAmount < 1 && decrementAmount > 0)
				{
					TryRemoveValue(currentValue * decrementAmount);
				}
				else
				{
					TryRemoveValue(decrementAmount);
				}
				break;
			case Enums.StatDayBehavior.INCREMENT:
				if (incrementAmount < 1 && incrementAmount > 0)
				{
					TryAddValue(currentValue * incrementAmount);
				}
				else
				{
					TryAddValue(incrementAmount);
				}
				break;
			case Enums.StatDayBehavior.MAX:
				SetValue(range.Y);
				break;
			case Enums.StatDayBehavior.MIN:
				SetValue(range.X);
				break;
		}
	}
}
