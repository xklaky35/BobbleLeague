using Godot;

namespace src.UI{
	public partial class PlayerProfile : Control
	{
		Godot.Label playerName;
		Godot.Label playerID;

		public override void _Ready(){
			playerName = GetNode<Godot.Label>("ColorRect/name_player");
			playerID = GetNode<Godot.Label>("ColorRect/id_player");
		}

		public void setName(string name){
			playerName.Text = name;
		}

		public void setId(int id){
			playerID.Text = id.ToString();
		}
	}
}

