using Godot;

public partial class player : RigidBody3D{

	private const float RayRange = 1000f;

	[Export]
	public int speed {get; set;}

	public bool is_pressed;

	public Vector3 direction;

	RigidBody3D arrow;
	Camera3D camera;
	MeshInstance3D arrow_body;
	Timer timer;


	// init camera
	public override void _Ready(){
		camera = GetNode<Camera3D>("../Camera3D");
		arrow_body = GetNode<MeshInstance3D>("Arrow/ArrowBody");
		arrow = GetNode<RigidBody3D>("Arrow");
		
		// connect player to the timeout event
		timer = GetNode<Timer>("../Timer");
		timer.Timeout += _on_timer_timeout;
	}

	// game loop
	public override void _PhysicsProcess(double delta){
		CheckMousePosition();
		arrow.Position = Position;
	}

	//add force to the 'Body element'
	private void POWER(){
		ApplyCentralImpulse(direction * speed);
		arrow.Visible = false;
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
}

