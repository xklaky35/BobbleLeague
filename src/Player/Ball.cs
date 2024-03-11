using Godot;
using src.GameLogic;

namespace src.Player{
	public partial class Ball : RigidBody3D{

		public Vector3 prevPos;
		public bool isMoving;


		public override void _Ready(){
			prevPos = GlobalPosition;
			isMoving = false;
		}

		public override void _PhysicsProcess(double delta){
			checkPlayerMovement();
		}

		private void checkPlayerMovement(){
			Vector3 movementCheck = (GlobalPosition - prevPos) * (new Vector3(1,0,1));
			if(movementCheck != new Vector3(0,0,0)){
				MainGame.ballMoving = true;
			}else{
				MainGame.ballMoving = false;
			}
			prevPos = GlobalPosition;
		}
	}
}

