using Godot;
using System;

public partial class SplashScreen : Control
{
    public override void _Ready()
    {
        SceneTransitionManager.Instance.TransitionToScene("StartMenu");
    }
}
