using Godot;
using src.Networking;

public partial class Goals : Node3D{
	private Area3D ScoreZoneRed;
	private Area3D ScoreZoneBlue;

	[Signal]
	public delegate void OnGoalEventHandler();

	public override void _Ready(){
		ScoreZoneRed = GetNode<Area3D>("ScoreZoneRed");
		ScoreZoneBlue = GetNode<Area3D>("ScoreZoneBlue");
	}

	private void _on_score_zone_red_body_entered(Node3D body){
		GD.Print(body.Name);
		if(body.IsInGroup("Ball")){
			GameManager.redScore++;
			GD.Print("Red Score: " + GameManager.redScore);
			EmitSignal("OnGoal");
		}
	}


	private void _on_score_zone_blue_body_entered(Node3D body){
		GD.Print(body.Name);
		if(body.IsInGroup("Ball")){
			GameManager.blueScore++;
			GD.Print("Blue Score: " + GameManager.blueScore);
			EmitSignal("OnGoal");
		}
	}

}


