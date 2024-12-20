using Godot;
using System;
using System.Collections.Generic;
public partial class DataBaseManager : Node
{
    public static DataBaseManager Instance;
    
    private string sLevelDataPath = "res://Data/Levels/";
    private Dictionary<String,LevelData> dictLevelDB = new Dictionary<String,LevelData>();
    
    public override void _Ready()
    {
        LoadLevelData();
        Instance = this;
    }
    private void LoadLevelData()
    {
        dictLevelDB.Clear();
        var levels = DirAccess.Open(sLevelDataPath);


        if (levels != null)
        {
            levels.ListDirBegin();
            var FileName = levels.GetNext();
            while (FileName != "")
            {

                if (FileName.EndsWith(".tres"))
                {
                    string sfullpath = sLevelDataPath + FileName;
                    var resource = GD.Load<LevelData>(sfullpath);
                    dictLevelDB[resource.sID] = resource;
                }

                FileName = levels.GetNext();
            }
            
        }
    }

    public LevelData GetLevel(string sLevelName)
    {
        LevelData levelData = dictLevelDB[sLevelName];
        return levelData;
    }
}
