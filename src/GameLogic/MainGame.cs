using Godot;

namespace src.GameLogic{
	public partial class MainGame : Node3D{
        
		private Timer timer;
		private Timer checkDelayTimer;

		private bool readyToCheck = false;

		public override void _Ready(){
			timer = GetNode<Timer>("Timer");
			checkDelayTimer = GetNode<Timer>("CheckDelayTimer");
		}

		public override void _Process(double delta){
			if(readyToCheck && GetTree().GetNodesInGroup("Moving").Count == 0){
				readyToCheck = false;
				timer.Start();
			}
		}

		private void _on_timer_timeout(){
			checkDelayTimer.Start();
		}

		private void _on_check_delay_timer_timeout(){
			readyToCheck = true;
		}
	}
}


