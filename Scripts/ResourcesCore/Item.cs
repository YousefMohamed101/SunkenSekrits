using Godot;
using System;

[GlobalClass]
public partial class Item : Resource
{
    [Export] public string sID;
    [Export] public string sName;
    [Export] public string sDescription;
    [Export] public Texture2D tex2dIcon;
    [Export] public PackedScene psScene;
    [Export] public bool bQuestItem;

    public virtual void Use()
    {
        GD.Print("Use");
    }
}
