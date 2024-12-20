using Godot;
using System;

public partial class Settings : Control
{
    [Signal]
    public delegate void SettingReturnEventHandler();

    private Button btnBack;
    public void Open()
    {
        this.Visible = true;
    }
    public void Close()
    {
        this.Visible = false;
    }

    public override void _Ready()
    {
        btnBack = GetNode<Button>("%SettingsBack");
        btnBack.Pressed += ()=> EmitSignal("SettingReturn");
    }
}
