using Godot;
using System;

public partial class KeyBind : BoxContainer
{
	public Label KeyBindLabel ;
	public Button KeyBindButton;

	public override void _Ready() {
		KeyBindLabel = GetNode<Label>("ActionLabel");
		KeyBindButton = GetNode<Button>("ActionBind");
	}
	
}
