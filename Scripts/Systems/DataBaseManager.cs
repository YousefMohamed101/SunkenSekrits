using Godot;
using System;
using System.Collections.Generic;


public partial class DataBaseManager : Node
{
    public static DataBaseManager Instance;
    

    private Dictionary<String,LevelData> _levelDb = new Dictionary<String,LevelData>();
    private string _levelDataResources = "res://Data/Levels/";
    public override void _Ready()
    {
        
        
        LoadLevelData();
        
        
        Instance = this;
    }
    private void LoadLevelData()
    {
        _levelDb.Clear();
        var levels = DirAccess.Open(_levelDataResources);


        if (levels != null)
        {
            levels.ListDirBegin();
            var fileName = levels.GetNext();
            while (fileName != "")
            {

                if (fileName.EndsWith(".tres"))
                {
                    string sfullpath = _levelDataResources + fileName;
                    var resource = GD.Load<LevelData>(sfullpath);
                    _levelDb[resource.Id] = resource;
                }

                fileName = levels.GetNext();
            }
            
        }
    }

    public LevelData GetLevel(string levelId)
    {
        LevelData levelData = _levelDb[levelId];
        return levelData;
    }
}
