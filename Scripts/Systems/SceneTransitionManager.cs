using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public partial class SceneTransitionManager : Node
{
    public static SceneTransitionManager Instance;
    
    private enum LoadingStates
    {
        Idle,
        Loading,
        Loaded,
        Error
    }

    
    private ColorRect crColorRect;
    private CanvasLayer clCanvasLayer;
    private AnimationPlayer apAnimPlayer;
    private LoadingStates eCurrentLoadingState = LoadingStates.Idle;
    private string sDataPath ;
    private PackedScene psLoadedScene;
    private LevelData lData;
    public override void _Ready()
    {
        PersistantLoad();
        Instance = this;
        
    }

    public async void TransitionToScene(string sSceneID)
    {
        crColorRect.Visible = true;
        lData = DataBaseManager.Instance.GetLevel(sSceneID);
        if (lData == null)
        {
            GD.PrintErr("Scene " + sSceneID + " not found");
            return;
        }
        apAnimPlayer.Play("FadeOut");
        await ToSignal(apAnimPlayer, "animation_finished");

        bool bLoadCheck = await LoadScene(lData.sScenePath);

        if (bLoadCheck && psLoadedScene != null)
        {
            
            GetTree().ChangeSceneToPacked(psLoadedScene);
            apAnimPlayer.Play("FadeIn");
            crColorRect.Visible = false;
        }
        else
        {
            apAnimPlayer.Play("FadeIn");
            GD.PrintErr("Failed to change the Scene to ",$"{sSceneID}");
            crColorRect.Visible = false;
        }
    }

    private async Task<bool> LoadScene(string sScenePath)
    {
        
        eCurrentLoadingState = LoadingStates.Loading;
        GD.Print("Loading " + sScenePath);
        try
        {
            
            ResourceLoader.LoadThreadedRequest(sScenePath);
            while (eCurrentLoadingState == LoadingStates.Loading)
            {
                
                var status = ResourceLoader.LoadThreadedGetStatus(sScenePath);
                
                if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                {
                    
                    eCurrentLoadingState = LoadingStates.Loaded;
                    psLoadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(sScenePath);
                    
                    return true;
                }

                if (status == ResourceLoader.ThreadLoadStatus.Failed)
                {
                    eCurrentLoadingState = LoadingStates.Error;
                    GD.PrintErr("Error loading scene: " + sScenePath);
                    return false;
                }
                if (status == ResourceLoader.ThreadLoadStatus.InvalidResource)
                {
                    eCurrentLoadingState = LoadingStates.Error;
                    GD.PrintErr("Invalid Resorce at: " + sScenePath);
                    return false;
                }
                if (status == ResourceLoader.ThreadLoadStatus.InProgress)
                {
                    eCurrentLoadingState = LoadingStates.Loading;
                    //GD.PrintErr("Resource still in process at" + sScenePath);
                    //await Task.Delay(10);
                }
            }

            
        } catch (Exception e)
        {
            GD.PrintErr($"Exception during scene loading: {e.Message}");
            eCurrentLoadingState = LoadingStates.Error;
            return false;
        }
        return false;
    }
    
    

 
    

    private void PersistantLoad()
    {
        clCanvasLayer = GetNode<CanvasLayer>("%CurtainHolder");
        clCanvasLayer.Layer = 100;
        crColorRect = GetNode<ColorRect>("%CurtainTransition");
        crColorRect.Modulate = new Color(Colors.Black, 0);
        crColorRect.Visible = false;
        apAnimPlayer = GetNode<AnimationPlayer>("%TranstionAnimationManager");

    }

    
}
