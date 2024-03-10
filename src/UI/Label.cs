using Godot;

public partial class Label : Godot.Label
{
	Timer timer;
	public override void _Ready(){
		timer = GetNode<Timer>("../../Timer");
		timer.Start();
	}

	public override void _Process(double delta){
		Text = ((int)timer.TimeLeft).ToString();
	}

	private void _on_timer_timeout(){
		timer.Stop();
	}
}


