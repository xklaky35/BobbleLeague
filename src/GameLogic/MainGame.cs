using Godot;


namespace src.GameLogic{
	public  partial class MainGame : Node3D{

		static public bool playerMoving;
		static public bool ballMoving;
		static public bool isPaused;

		public Timer timer;

		public override void _Ready(){
			timer = GetNode<Timer>("Timer");
		}

		public override void _Process(double delta){

			// only if the timer is stopped and the ball and players are not moving the timer will restart
			if(timer.IsStopped() && !ballMoving && !playerMoving){
				timer.Start();
				isPaused = false;
			}

		}
		private void _on_timer_timeout(){
			//pause the timer until nothing is moving
			isPaused = true;
		}
	}
}








