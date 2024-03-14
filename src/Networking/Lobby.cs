using Godot;
using System.Collections.Generic;
using src.Player;


namespace src.Networking{
	public partial class Lobby : Control
	{
		[Export]
		private const int PORT = 8910;
		[Export]
		private const string IP_ADDRESS = "127.0.0.1";
		[Export]
		private const int MAX_CLIENTS = 6;

		[Signal]
		public delegate void PlayerConnectedEventHandler(int peer_id);
		[Signal]
		public delegate void PlayerDisconnectedEventHandler(int peer_id);
		[Signal]
		public delegate void ServerDisconnectedEventHandler();

		[Signal]
		public delegate void PlayerSwitchedToBlueEventHandler();
		[Signal]
		public delegate void PlayerSwitchedToRedEventHandler();



		public override void _Ready(){
			Multiplayer.PeerConnected += OnPlayerConnected;
			Multiplayer.PeerDisconnected += OnPlayerDisconnected;
			Multiplayer.ConnectedToServer += OnServerConnectionOK;
			Multiplayer.ConnectionFailed += OnServerConnectionFailed;
			Multiplayer.ServerDisconnected += OnServerDisconnected;
		}

		// ### NETWORK SIGNALS ###


		// called on all the peers
		// id is the peer connecting
		private void OnPlayerConnected(long id){
			GD.Print("Client " + id + " connected!(ALL)" + Multiplayer.GetUniqueId());
			EmitSignal("PlayerConnected", id);
		}

		// called on all the peers
		// id is the peer disconnecting
		private void OnPlayerDisconnected(long id){
			GD.Print("Client " + id + " disconnected!(ALL)" + Multiplayer.GetUniqueId());
			EmitSignal("PlayerDisconnected", id);
		}

		// only on client
		private void OnServerDisconnected(){
			GD.Print("Disconnected from server: " + Multiplayer.GetUniqueId());
		}

		// only on client
		private void OnServerConnectionOK(){
			GD.Print("To Server Connected: " + Multiplayer.GetUniqueId());
			RpcId(1, "_register_player", Multiplayer.GetUniqueId(), GetNode<LineEdit>("LineEdit").Text);
		}

		// only on client
		private void OnServerConnectionFailed(){
			GD.Print("Connection to server failed: " + Multiplayer.GetUniqueId());
		}


		// ##### BUTTONS ON THE UI #####

		// create game
		// start a multiplayer server
		private void _on_host_button_down(){
			ENetMultiplayerPeer host = new ENetMultiplayerPeer();
			host.CreateServer(PORT, MAX_CLIENTS);
			Multiplayer.MultiplayerPeer = host;
			GD.Print("Server Created");

			_register_player(Multiplayer.GetUniqueId(), GetNode<LineEdit>("LineEdit").Text);	
		}


		// join a existing multiplayer server
		private void _on_join_button_down(){
			ENetMultiplayerPeer client = new ENetMultiplayerPeer();
			client.CreateClient(IP_ADDRESS, PORT);
			Multiplayer.MultiplayerPeer = client;
			GD.Print("Client Created");
		}


		private void _on_switch_to_blue_button_down()
		{

			if(Multiplayer.GetUniqueId() != 1){
				//the player has to exist in the playerData list
				foreach(var item in GameManager.playerData){
					//needed to access the name
					if(item.player_id == Multiplayer.GetUniqueId()){
						RpcId(1, "_addToBlueTeam", item.player_id, item.player_name);
					}
				}
			}else{
				foreach(var item in GameManager.playerData){
					//needed to access the name
					if(item.player_id == Multiplayer.GetUniqueId()){
						_addToBlueTeam(item.player_id, item.player_name);
					}
				}
			}

		}


		private void _on_switch_to_red_button_down()
		{
			//if you are not the server send him data
			if(Multiplayer.GetUniqueId() != 1){
				//needed to access the name
				foreach(var item in GameManager.playerData){
					//the player has to exist in the playerData list
					if(item.player_id == Multiplayer.GetUniqueId()){
						RpcId(1, "_addToRedTeam", item.player_id, item.player_name);
					}
				}
			}else{
				foreach(var item in GameManager.playerData){
					//needed to access the name
					if(item.player_id == Multiplayer.GetUniqueId()){
						_addToRedTeam(item.player_id, item.player_name);
					}
				}
			}
		}


		// starts the main game
		private void _on_start_button_down(){
			//only the host can start the game
			if(Multiplayer.GetUniqueId() == 1){
				Rpc("_start_game");
			}else{
				GD.Print("Wait for the host to start!");
			}

		}




		// ##### RPC CALLS #####
		
		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
		private void _start_game(){
			var scene = GD.Load<PackedScene>("res://Scenes/main_game.tscn").Instantiate<Node3D>();
			GetTree().Root.AddChild(scene);
			this.Hide();
		}


		//register player data when connecting
		[Rpc(MultiplayerApi.RpcMode.AnyPeer)] 
		private void _register_player(int playerID, string player_name){
			// create a new blob of data for the host
			PlayerInfo playerInfo = new PlayerInfo{
				player_id = playerID,
				player_name = player_name,
			};
			if(!GameManager.playerData.Contains(playerInfo)){
				GameManager.playerData.Add(playerInfo);
			}

			// send the data to all players
			if(Multiplayer.IsServer()){
				foreach(var player in GameManager.playerData){
					Rpc("_register_player", player.player_id, player.player_name);
					showLobby();
				}
				foreach(var player in GameManager.bluePlayerData){
					Rpc("_syncTeamState", player.player_id, player.player_name, "blue");
				}
				foreach(var player in GameManager.redPlayerData){
					Rpc("_syncTeamState", player.player_id, player.player_name, "red");
				}
				EmitSignal("PlayerConnected", playerID);
			}
		}


		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		private void showLobby(){
			foreach(var item in GameManager.playerData){
				GD.Print("Main" + item.player_name + " : " + item.player_id);
			}

			//Print blue team
			foreach(var item in GameManager.bluePlayerData){
				GD.Print("Blue" + item.player_name + " : " + item.player_id);
			}

			//Print red team
			foreach(var item in GameManager.redPlayerData){
				GD.Print("Red" + item.player_name + " : " + item.player_id);
			}
		}


		//add player data to blue team if selected
		[Rpc(MultiplayerApi.RpcMode.AnyPeer)] 
		private void _addToBlueTeam(int playerID, string player_name){
			PlayerInfo playerInfo = new PlayerInfo{
				player_id = playerID,
				player_name = player_name,
			};

			bool alreadyInBlue = false;

			//check if player already is in blue team
			foreach(var item in GameManager.bluePlayerData){
				if(item.player_id == playerInfo.player_id) alreadyInBlue = true;
			}
			if(alreadyInBlue == false) GameManager.bluePlayerData.Add(playerInfo);

			bool inRedBefore = false;
			//check if player was in red team before
			foreach(var item in GameManager.redPlayerData){
				if(item.player_id == playerInfo.player_id) inRedBefore = true;
			}
			if(inRedBefore == true){
				foreach(var item in GameManager.redPlayerData){
					if(item.player_id == playerInfo.player_id){
						GameManager.redPlayerData.Remove(item);
						break;
					}               }
			}

			if(Multiplayer.IsServer()){
				foreach(var item in GameManager.bluePlayerData){
					Rpc("_addToBlueTeam", item.player_id, item.player_name);
					showLobby();
				}
			}
			EmitSignal("PlayerSwitchedToBlue");
		}


		//add player data to red team if selected
		[Rpc(MultiplayerApi.RpcMode.AnyPeer)] 
		private void _addToRedTeam(int playerID, string player_name){
			PlayerInfo playerInfo = new PlayerInfo{
				player_id = playerID,
				player_name = player_name,
			};

			bool alreadyInRed = false;

			//check if player already is in red team
			foreach(var item in GameManager.redPlayerData){
				if(item.player_id == playerInfo.player_id) alreadyInRed = true;
			}

			if(alreadyInRed == false)  GameManager.redPlayerData.Add(playerInfo);

			bool inBlueBefore = false;
			//check if player was in blue team before
			foreach(var item in GameManager.bluePlayerData){
				if(item.player_id == playerInfo.player_id) inBlueBefore = true;
			}

			if(inBlueBefore == true){
				foreach(var item in GameManager.bluePlayerData){
					if(item.player_id == playerInfo.player_id){
						GameManager.bluePlayerData.Remove(item);
						break;
					}                }
			}

			if(Multiplayer.IsServer()){
				foreach(var item in GameManager.redPlayerData){
					Rpc("_addToRedTeam", item.player_id, item.player_name);
					showLobby();
				}
			}
			EmitSignal("PlayerSwitchedToRed");
		}
		
		[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
		private void _syncTeamState(int playerID, string playerName, string team){
			if(!Multiplayer.IsServer()){
				if(team == "blue"){
					//sync blue team data
					GameManager.bluePlayerData.Add(new PlayerInfo{player_id = playerID, player_name = playerName});
				}else{
					//sync red team data
					GameManager.redPlayerData.Add(new PlayerInfo{player_id = playerID, player_name = playerName});
				}
			}
			EmitSignal("PlayerConnected", playerID);
			// the server has to send all its data to the clients
			if(Multiplayer.IsServer()){

			}
		}
	}
}




