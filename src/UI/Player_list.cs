using Godot;
using System.Collections.Generic;
using src.Networking;
using src.Player;

namespace src.UI{
	public partial class Player_list : Control
	{
		[Export]
		private PackedScene player_profile;

		[Export]
		private float offset_inkrement = 90f;

		private Control playerListMain;
		private Control playerListBlue;
		private Control playerListRed;

		public override void _Ready(){
			playerListMain = GetNode<Control>("PlayerListMain");
			playerListBlue = GetNode<Control>("PlayerListBlue");
			playerListRed = GetNode<Control>("PlayerListRed");
		}


		private void _on_lobby_player_connected(long peer_id){

			_removePlayerProfiles();
			//_createPlayerProfiles(playerListMain, "main");
			_createPlayerProfiles(playerListBlue, "blue");
			_createPlayerProfiles(playerListRed, "red");
		}


		private void _on_lobby_player_disconnected(long peer_id){

			_removePlayerProfiles();
			//_createPlayerProfiles(playerListMain, "main");

		}


		private void _on_lobby_player_switched_to_blue(){
			_removePlayerProfiles();
			//_createPlayerProfiles(playerListMain, "main");
			_createPlayerProfiles(playerListBlue, "blue");
			_createPlayerProfiles(playerListRed, "red");
		}


		private void _on_lobby_player_switched_to_red(){

			_removePlayerProfiles();
			//_createPlayerProfiles(playerListMain, "main");
			_createPlayerProfiles(playerListBlue, "blue");
			_createPlayerProfiles(playerListRed, "red");
		}



		private void _createPlayerProfiles(Control target, string team){
			float offset = 0f;

			List<PlayerInfo> playerList;

			switch(team){
				case "blue":
					playerList = GameManager.bluePlayerData;
					break;
				case "red":
					playerList = GameManager.redPlayerData;
					break;
				default:
					playerList = GameManager.playerData;
					break;
			}


			foreach(var item in playerList){

				var current_player_profile = player_profile.Instantiate<PlayerProfile>();
				current_player_profile.Name = item.player_id.ToString();
				current_player_profile.AddToGroup("player_profiles");
				target.AddChild(current_player_profile);

				//configure profile
				current_player_profile.setName(item.player_name);
				current_player_profile.setId(item.player_id);
				current_player_profile.GlobalPosition = target.GlobalPosition;

				//update profile position
				Vector2 profilePosition = new Vector2(0, offset);
				current_player_profile.GlobalPosition += profilePosition;

				offset += offset_inkrement;
			}
		}


		private void _removePlayerProfiles(){
			foreach(var item in GetTree().GetNodesInGroup("player_profiles")){
				item.QueueFree();
			}
		}
	}
}




