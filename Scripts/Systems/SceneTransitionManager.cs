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

    private Dictionary<String,LevelData> dictSceneCataloge = new Dictionary<String,LevelData>();
    private ColorRect crColorRect;
    private CanvasLayer clCanvasLayer;
    private AnimationPlayer apAnimPlayer;
    private LoadingStates eCurrentLoadingState = LoadingStates.Idle;
    private string sDataPath = "res://Data/Levels/";
    private PackedScene psLoadedScene;
    
    public override void _Ready()
    {
        PersistantLoad();
        LoadScenesDB();
        Instance = this;
        
    }

    public async void TransitionToScene(string sSceneID)
    {
        crColorRect.Visible = true;
        if (!dictSceneCataloge.ContainsKey(sSceneID))
        {
            GD.PrintErr("Scene " + sSceneID + " not found");
            return;
        }
        apAnimPlayer.Play("FadeOut");
        await ToSignal(apAnimPlayer, "animation_finished");

        bool bLoadCheck = await LoadScene(dictSceneCataloge[sSceneID].sScenePath);

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
                    GD.PrintErr("Resource still in process at" + sScenePath);
                    
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
    
    

 
    private void LoadScenesDB()
    {
        dictSceneCataloge.Clear();
        var levels = DirAccess.Open(sDataPath);


        if (levels != null)
        {
            levels.ListDirBegin();
            var FileName = levels.GetNext();
            while (FileName != "")
            {

                if (FileName.EndsWith(".tres"))
                {
                    string sfullpath = sDataPath + FileName;
                    var resource = GD.Load<LevelData>(sfullpath);
                    dictSceneCataloge[resource.sID] = resource;
                }

                FileName = levels.GetNext();
            }
            
        }
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
