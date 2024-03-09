using Godot;
using src.Player;

namespace src.Networking{
	public partial class PlayerManager : Node3D
	{
		[Export]
		private PackedScene playerScene;

		public override void _Ready(){
			int index = 0;


			//for every player in the playerData list
			foreach(var item in GameManager.playerData){


				//create 3 players for each player in the list
				for(int i = 0; i < 3; i++){
					PlayerClass currentPlayer = playerScene.Instantiate<PlayerClass>();
					currentPlayer.Name = "Player" + item.player_id.ToString();
					AddChild(currentPlayer);

					

					foreach(Node3D playerSide in GetTree().GetNodesInGroup("PlayerSide")){
						if(int.Parse(playerSide.Name) == index){
							foreach(Node3D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoint")){
								if(int.Parse(spawnPoint.Name) == i){
									currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
								}
							}
						}
					}
				}
				index++;
			}
		}

		private void spawnBlueSide(){}
		private void spawnRedSide(){}
	}


}

