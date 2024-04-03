using Godot;

namespace src.Player{
	public partial class Ball : RigidBody3D{

		public Vector3 prevPos;
		public bool isMoving;


		public override void _Ready(){
			prevPos = GlobalPosition;
			isMoving = false;
			AddToGroup("Ball");
		}

		public override void _PhysicsProcess(double delta){
			checkPlayerMovement();
		}

		private void checkPlayerMovement(){
			Vector3 movementCheck = (GlobalPosition - prevPos) * (new Vector3(1,0,1));
			if(movementCheck != new Vector3(0,0,0)){
				isMoving = true;
				AddToGroup("Moving");
			}else{
				isMoving = false;
				RemoveFromGroup("Moving");
			}
			prevPos = GlobalPosition;
		}
	}
}

