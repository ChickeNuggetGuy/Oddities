using Godot;
using System;

[GlobalClass]
public partial class Stat : Resource
{
	[Export] public Enums.StatType statType;
	[Export] public double currentValue;
	[Export] public Vector2I range;
	
	[Export] public bool signalOnMin;
	[Export] public bool signalOnMax;
	
	[Signal] public delegate void ValueChangedEventHandler(double currentValue, Vector2I range);
	[Signal] public delegate void MinValueReachedEventHandler(double currentValue, Vector2I range);
	[Signal] public delegate void MaxValueReachedEventHandler(double currentValue, Vector2I range);


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
}
