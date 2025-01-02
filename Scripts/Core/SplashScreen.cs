using Godot;
using System;

public partial class SplashScreen : Control
{
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("LogoFade");
        GetNode<Timer>("TranstionStarter").Timeout += ()=> SceneTransitionManager.Instance.TransitionToScene("StartMenu");
    }
}
