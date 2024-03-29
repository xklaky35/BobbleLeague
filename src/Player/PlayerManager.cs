using Godot;
using Godot.Collections;
using src.Networking;

namespace src.Player{

	public partial class PlayerManager : Node3D
	{
		[Export]
		private PackedScene playerScene;

		public override void _Ready(){

			spawnBlueSide();
			spawnRedSide();
			addBlueRights();
			addRedRights();

		}

		private void spawnBlueSide(){

			int index = 0;

			foreach(Node3D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnBlue")){
				PlayerClass currentPlayer = playerScene.Instantiate<PlayerClass>();
				currentPlayer.Name = "BluePlayer_" + index;
				currentPlayer.AddToGroup("BluePlayer");
				currentPlayer.AddToGroup("Player");


				AddChild(currentPlayer);

				if(int.Parse(spawnPoint.Name) == index){
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
					currentPlayer.prevPos = spawnPoint.GlobalPosition;
				}
				index++;
			}
		}


		private void spawnRedSide(){

			int index = 0;

			foreach(Node3D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnRed")){
				PlayerClass currentPlayer = playerScene.Instantiate<PlayerClass>();
				currentPlayer.Name = "RedPlayer_" + index;
				currentPlayer.AddToGroup("RedPlayer");
				currentPlayer.AddToGroup("Player");
				
				AddChild(currentPlayer);

				if(int.Parse(spawnPoint.Name) == index){
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
					currentPlayer.prevPos = spawnPoint.GlobalPosition;
				}
				index++;
			}
		}

		private void addBlueRights(){
			Array<Node> bluePlayers = GetTree().GetNodesInGroup("BluePlayer");

			int playerIndex = 0;
			int bluePlayerCount = GameManager.bluePlayerData.Count; 
			GD.Print("BluePlayer: " + bluePlayerCount);
			foreach(var player in bluePlayers){
				//depending on how many players are in the blue team(index%anzahl) add rights for the players to the objects
				player.GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(GameManager.bluePlayerData[playerIndex%bluePlayerCount].player_id);
				playerIndex++;
				GD.Print(player.Name + "-> ID: " + GameManager.bluePlayerData[playerIndex%bluePlayerCount].player_id);
			}

		}
		private void addRedRights(){
			Array<Node> redPlayers = GetTree().GetNodesInGroup("RedPlayer");

			int playerIndex = 0;
			int redPlayerCount = GameManager.redPlayerData.Count;
			GD.Print("RedPlayer: " + redPlayerCount);
			foreach(var player in redPlayers){
				//depending on how many players are in the red team(index%anzahl) add rights for the players to the objects
				player.GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(GameManager.redPlayerData[playerIndex%redPlayerCount].player_id);
				playerIndex++;
				GD.Print(player.Name + "-> ID: " + GameManager.redPlayerData[playerIndex%redPlayerCount].player_id);
			}

		}
	}
}

