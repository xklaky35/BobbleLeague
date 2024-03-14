using Godot;
using src.GameLogic;
using src.Networking;

namespace src.Player{
	public partial class PlayerClass : RigidBody3D{

		private const float RayRange = 1000f;

		[Export]
		public int speed = 5;
		public bool isMoving;

		bool is_pressed;
		public Vector3 prevPos;

		Vector3 direction;

		RigidBody3D arrow;
		Camera3D camera;
		MeshInstance3D arrow_body;
		Timer timer;

		[Signal]
		public delegate void IsMovingEventHandler();

		public override void _Ready(){
			camera = GetNode<Camera3D>("/root/Main/Camera3D");
			arrow_body = GetNode<MeshInstance3D>("Arrow/ArrowBody");
			arrow = GetNode<RigidBody3D>("Arrow");

			// connect player to the timeout event
			timer = GetNode<Timer>("/root/Main/Timer");
			timer.Timeout += _on_timer_timeout;
			isMoving = false;
		}


		public override void _PhysicsProcess(double delta){
			if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
				CheckMousePosition();
				arrow.Position = Position;
				arrow.Show();
			}else{
				arrow.Hide();
			}

			//if(MainGame.isPaused){
			//	arrow.Hide();
			//}else{
			//	arrowTeamVisibility();
			//}
		}

		//add force to the 'Body element'
		private void POWER(){
			ApplyCentralImpulse(direction * speed);
		}



		private void CheckMousePosition(){
			//mouse pos is needed every frame if it is presseddictionary is empty
			var mouse_position = GetViewport().GetMousePosition();

			//ray cast origin (camera)
			Vector3 origin = camera.ProjectRayOrigin(mouse_position);
			//ray cast target
			Vector3 target = origin + camera.ProjectRayNormal(mouse_position)*RayRange; 

			var query = PhysicsRayQueryParameters3D.Create(origin, target);
			var intersection = GetWorld3D().DirectSpaceState.IntersectRay(query);

			if(intersection.ContainsKey("position")){

				var exact_looking_pos = (Vector3)intersection["position"];

				//needed for .LookAt()
				Vector3 looking_pos = new Vector3(exact_looking_pos.X, GetNode<RigidBody3D>("Arrow").Position.Y, exact_looking_pos.Z);
				Vector3 up_vector = new Vector3(0,1,0);

				if(is_pressed){
					arrow_body.LookAt(looking_pos,up_vector);

					// vector from the player to the mouse
					Vector3 arrow_vector = looking_pos - Position;
					direction = arrow_vector;

					//change the length of the arrow
					Vector3 length_vector = new Vector3(arrow_body.Scale.X, arrow_body.Scale.Y,arrow_vector.Length());
					arrow_body.Scale = length_vector;
				}
			}

		}


		private void checkPlayerMovement(){
			Vector3 movementCheck = (GlobalPosition - prevPos) * (new Vector3(1,0,1));
			if(movementCheck != new Vector3(0,0,0)){
				MainGame.playerMoving = true;
			}else{
				MainGame.playerMoving = false;
			}

			prevPos = GlobalPosition;
		}


		private void _on_timer_timeout(){
			POWER();
		}

		private void _on_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shape_idx){
			//click on the player body
			if(@event is InputEventMouseButton mouseButton && mouseButton.Pressed){
				GD.Print("Mouse Pressed on arrow");
				is_pressed = true;
			}
		}

		// Handle overall input 
		public override void _Input(InputEvent @event){
			//mouse button released
			if(@event is InputEventMouseButton mouseButton && mouseButton.Pressed == false){
				is_pressed = false;
			}
		}

		private void arrowTeamVisibility(){
			if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
				CheckMousePosition();
				arrow.Position = Position;
				arrow.Show();
			}
			// if this the authority of this player is in the the red team and this 
			//foreach(var player in GameManager.redPlayerData){
			//	if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == player.player_id) {
			//		if(IsInGroup("RedPlayer")){
			//			arrow.Show();
			//		}else{
			//			arrow.Hide();
			//		}
			//	}
			//}
			//foreach(var player in GameManager.bluePlayerData){
			//	if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == player.player_id) {
			//		if(IsInGroup("BluePlayer")){
			//			arrow.Show();
			//		}else{
			//			arrow.Hide();
			//		}
			//	}
			//}
		}
	}
}


