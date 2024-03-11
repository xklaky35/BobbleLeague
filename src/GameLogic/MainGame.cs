using Godot;
using System;


namespace src.GameLogic{
	public  partial class MainGame : Node3D{

		static public bool movement;
		static public bool ballMoving;

		public override void _Process(double delta){
			if(ballMoving){
				//GD.Print("Ball moving");
			}

			if(movement){
				GD.Print("Player Moving");
			}
		}
	}
}




