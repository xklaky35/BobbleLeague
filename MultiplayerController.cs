using Godot;

public partial class MultiplayerController : Control{
    [Export]
    private int port = 8910;
    [Export]
    private string address = "127.0.0.1";
    private ENetMultiplayerPeer peer;

    public override void _Ready(){
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
    }


    // runs when player connects on all peers
    // id of the player that connects
    public void PeerConnected(long id){
        GD.Print("Player connected: " + id.ToString());
    }

    // runs when player disconnects on all peers
    // id of the player that disconnects
    public void PeerDisconnected(long id){
        GD.Print("Player disconnected: " + id.ToString());
    }
    // runs when connected to server on client
    public void ConnectedToServer(){
        GD.Print("Connected to server");
        Rpc("sendPlayerInformation", GetNode<LineEdit>("LineEdit").Text, Multiplayer.GetUniqueId());
    }

    // only runs when connection failed on the client
    public void ConnectionFailed(){
        GD.Print("Connection failed!"); 
    }

    private void _on_host_button_down(){
        // Replace with function body.
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(port, 4);

        if(error != Error.Ok){
            GD.Print("Error: " + error.ToString());
            return;
        }
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Waiting for players...");
    }


    private void _on_join_button_down(){
        // Replace with function body.
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(address, port);
        
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Joining Game");
    }


    private void _on_start_game_button_down(){
        // Replace with function body.
        Rpc("startGame");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void startGame(){
        var scene = ResourceLoader.Load<PackedScene>("res://main_game.tscn").Instantiate<Node3D>();
        GetTree().Root.AddChild(scene);
        this.Hide();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]  
    private void sendPlayerInformation(string name, int id){
        PlayerInfo playerInfo = new PlayerInfo(){
            Name = name,
            Id = id
        };
        if(!GameManager.Players.Contains(playerInfo)){
            GameManager.Players.Add(playerInfo);
        }
        if(Multiplayer.IsServer()){
            foreach(var item in GameManager.Players){
                Rpc("sendPlayerInformation",name, id);
            }
        }

    }
}


