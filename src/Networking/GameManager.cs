using Godot;
using System.Collections.Generic;
using src.Player;

namespace src.Networking{
	public partial class GameManager : Node{
		public static List<PlayerInfo> playerData = new List<PlayerInfo>();
		public static List<PlayerInfo> bluePlayerData = new List<PlayerInfo>();
		public static List<PlayerInfo> redPlayerData = new List<PlayerInfo>();
	}
}

