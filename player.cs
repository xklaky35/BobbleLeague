using Godot;

public partial class player : Node3D{

	

    private const float RayRange = 10f;

	[Export]
	public int speed {get; set;}

	public bool is_pressed;

	public Vector3 direction;

    //add force to the 'Body element'
	private void POWER(){
		var body = GetNode<RigidBody3D>("Body");
		body.ApplyCentralImpulse(direction * speed);
	}


	private void _on_timer_timeout(){
		POWER();
	}

	private void _on_body_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shape_idx){
        
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

        //mouse moved while body clicked
        if(@event is InputEventMouseMotion mouseMotion){
            if(is_pressed){
                var camera = GetNode<Camera3D>("../Camera3D");

                Vector3 origin = camera.ProjectRayOrigin(mouseMotion.Position);
                GD.Print(origin);

                Vector3 target = origin + camera.ProjectRayNormal(mouseMotion.Position)*RayRange; 
                GD.Print(target);
                
                var query = PhysicsRayQueryParameters3D.Create(origin, target);

                var intersection = GetWorld3D().DirectSpaceState.IntersectRay(query);
                var exact_looking_pos = (Vector3)intersection["position"];

                Vector3 looking_pos = new Vector3(exact_looking_pos.X, Position.Y, exact_looking_pos.Z);
                
                Vector3 up_vector = new Vector3(0,1,0);

                GetNode<MeshInstance3D>("Arrow").LookAt(looking_pos,up_vector);


            }
        }
	}
}

