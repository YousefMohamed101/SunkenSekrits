using Godot;
using System;

public partial class MainMenu : Control
{
    private Button btnNewGame;
    private Button btnLoadGame;
    private Button btnSaveGame;
    private Button btnSettings;
    private Button btnQuitGame;
    private Settings settSettings;
    private CenterContainer ccMenuHub;


    public override void _Ready()
    {
        btnNewGame = GetNode<Button>("%NewGame");
        btnLoadGame = GetNode<Button>("%LoadGame");
        btnSaveGame = GetNode<Button>("%SaveGame");
        btnSettings = GetNode<Button>("%SettingsButton");
        btnQuitGame = GetNode<Button>("%QuitGame");
        ccMenuHub = GetNode<CenterContainer>("%MenuHub");
        var ComponentCheck = GetChildren();
        foreach (var component in ComponentCheck)
        {
            if (component is Settings)
            {
                settSettings = GetNode<Settings>("%Settings");
                btnSettings.Pressed += GetSettingsHandling;
                btnSettings.Visible = true;
            }
            else
            {
                btnSettings.Visible = false;
            }
        }
        
       
        btnQuitGame.Pressed += ()=> GetTree().Quit();
    }

    private void GetSettingsHandling()
    {
        ccMenuHub.Visible = false;
        settSettings.Open();
    }
    
}
