using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class DataBaseManager : Node {
	public static DataBaseManager Instance;
	private string _levelDataResources = "res://Data/Levels/";

	private Dictionary<String, LevelData> _levelDb = new Dictionary<String, LevelData>();
	public List<AudioStream> AmbienceStream = new List<AudioStream>();
	public List<AudioStream> BackgroundStream = new List<AudioStream>();
	public List<AudioStream> SoundFXStream = new List<AudioStream>();
	public Dictionary<string, Dictionary<string, AudioStream[]>> StreamLibrary;

	public override void _Ready() {
		LoadLevelData();

		var walking = ResourceLoader.Load<StreamLibrary>("res://Assets/Scenes/Player/Data/NormalWalk.tres");
		var Sand = ResourceLoader.Load<StreamLibrary>("res://Assets/Scenes/Player/Data/SandFootStep.tres");
		var Water = ResourceLoader.Load<StreamLibrary>("res://Assets/Scenes/Player/Data/Water.tres");


		StreamLibrary = new Dictionary<string, Dictionary<string, AudioStream[]>>();
		var normalDict = new Dictionary<string, AudioStream[]> {
				["Running"] = walking.Running, ["Walking"] = walking.Walking, ["JumpingStart"] = walking.Jumpingstart, ["JumpingLand"] = walking.Jumpingland
		};
		var sandDict = new Dictionary<string, AudioStream[]> {
				["Running"] = Sand.Running, ["Walking"] = Sand.Walking, ["JumpingStart"] = Sand.Jumpingstart, ["JumpingLand"] = Sand.Jumpingland
		};
		var waterDict = new Dictionary<string, AudioStream[]> {
				["Running"] = Water.Running, ["Walking"] = Water.Walking, ["JumpingStart"] = Water.Jumpingstart, ["JumpingLand"] = Water.Jumpingland
		};

		StreamLibrary["Normal"] = normalDict;
		StreamLibrary["Sand"] = sandDict;
		StreamLibrary["Water"] = waterDict;

		Instance = this;
	}


	private void LoadLevelData() {
		_levelDb.Clear();
		var levels = DirAccess.Open(_levelDataResources);


		if(levels != null) {
			levels.ListDirBegin();
			var fileName = levels.GetNext();
			while(fileName != "") {
				if(fileName.EndsWith(".tres")) {
					string sfullpath = _levelDataResources + fileName;
					var resource = GD.Load<LevelData>(sfullpath);
					_levelDb[resource.Id] = resource;
				}

				fileName = levels.GetNext();
			}
		}
	}

	public LevelData GetLevel(string levelId) {
		LevelData levelData = _levelDb[levelId];
		return levelData;
	}
}