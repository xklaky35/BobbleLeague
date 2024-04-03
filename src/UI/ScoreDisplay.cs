using Godot;
using src.Networking;

public partial class ScoreDisplay : Control{

	Godot.Label blueScoreLabel;
	Godot.Label redScoreLabel;




	public override void _Ready(){
		blueScoreLabel = GetNode<Godot.Label>("BlueScore");
		redScoreLabel = GetNode<Godot.Label>("RedScore");
	}

	public override void _Process(double delta){
		blueScoreLabel.Text = GameManager.blueScore.ToString();
		redScoreLabel.Text = GameManager.redScore.ToString();
	}

}
