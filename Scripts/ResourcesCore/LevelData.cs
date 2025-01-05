using Godot;
using System;

[GlobalClass]
public partial class LevelData : Resource {
	[Export] public String Id;
	[Export] public String ScenePath;

	public string GetId() { return Id; }
}