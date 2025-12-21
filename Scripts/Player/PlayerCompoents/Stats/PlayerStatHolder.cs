using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class PlayerStatHolder : PlayerComponent
{
	[Export] public Dictionary<Enums.StatType, Stat> stats;
}
