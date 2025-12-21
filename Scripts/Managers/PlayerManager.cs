using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class PlayerManager : Manager<PlayerManager>
{
	[Export] private Dictionary<int, Player> players = new Dictionary<int, Player>();


	public bool TryAddplayer(int id, Player player)
	{
		if (players.ContainsKey(id)) return false;
		players.Add(id, player);
		return true;
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		if (players == null)
		{
			players = new Dictionary<int, Player>();
		}
	}

	protected override void Setup()
	{
		GD.Print( "playermanager Setup");
		if (!this.TryGetComponentsInChildrenRecursive<Player>(out var players))
		{
			GD.Print("No players found");
			return;
		}

		int playerId = 0;
		foreach (var player in players)
		{
			player.Setup(playerId);
			playerId++;
		}
	}
}
