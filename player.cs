using Godot;
using System;

public partial class player : Node3D{

	
	RigidBody3D arrow;
	RigidBody3D body;

	Marker3D arrow_end;
	Marker3D arrow_tip;

	[Export]
	public int speed {get; set;}


	public override void _Ready(){
		arrow = GetNode<RigidBody3D>("Arrow");
		body = GetNode<RigidBody3D>("Body");
		arrow_end = GetNode<Marker3D>("Arrow/Arrow_end");
		arrow_tip = GetNode<Marker3D>("Arrow/Arrow_tip");
	}

	public override void _PhysicsProcess(double delta){
		arrow.Position = body.Position;
	}
	
	private void _on_arrow_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shape_idx){
		if(@event is InputEventMouseButton mouseButton && mouseButton.Pressed){
			GD.Print("Mouse Pressed on arrow");
		}
	}

	private void POWER(){

		Vector3 new_dir =  arrow_tip.GlobalPosition - arrow_end.GlobalPosition;

		body.ApplyCentralImpulse(new_dir * speed); 
	}


	private void _on_timer_timeout(){
		POWER();
	}

}





