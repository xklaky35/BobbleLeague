using Godot;


public partial class lobby : Control
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
	}

	// called on all the peers
	// id is the peer disconnecting
	private void OnPlayerDisconnected(long id){
		GD.Print("Client " + id + " disconnected!(ALL)" + Multiplayer.GetUniqueId());
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


// #### BUTTONS ON THE UI ####

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

	// only doable by the host/server
	// starts the main game
	private void _on_start_button_down(){

	}

// ### RPC CALLS ###
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

		if(Multiplayer.IsServer()){
			foreach(var item in GameManager.playerData){
				Rpc("_register_player", playerID, player_name);
				showLobby();
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void showLobby(){
		foreach(var item in GameManager.playerData){
			GD.Print(item.player_name + " : " + item.player_id);
		}
	}
}
