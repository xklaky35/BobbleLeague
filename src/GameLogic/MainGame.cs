using Godot;
using src.Player;

namespace src.GameLogic{
	public partial class MainGame : Node3D{

		static public bool isTimerPaused;

		public Timer timer;

		public override void _Ready(){
			timer = GetNode<Timer>("Timer");
		}

		public override void _Process(double delta){

        }


}








