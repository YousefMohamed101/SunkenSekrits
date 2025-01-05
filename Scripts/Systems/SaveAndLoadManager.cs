using Godot;

public partial class SaveAndLoadManager : Node
{
	public static SaveAndLoadManager Instance;
	private SettingData _defaultSetting = GD.Load<SettingData>("res://Data/SettingsData/DefaultSetting.tres");
	private SettingData _userSetting;
	private string _userSettingPath= "user://Data/UserSetting.tres";
	private string _dataFolderPath= "user://Data";
	public override void _Ready()
	{
		InitializeUserData();
		Instance = this;
	}


	private void InitializeUserData()
	{
		
		if (DirAccess.DirExistsAbsolute(_dataFolderPath) != true)
		{
			DirAccess.MakeDirAbsolute(_dataFolderPath);
		}

		if (FileAccess.FileExists(_userSettingPath) != true)
		{
			_userSetting = _defaultSetting;
			_userSetting.ID = "UserSetting";
			ResourceSaver.Save(_userSetting, _userSettingPath);
		}
		else
		{
			_userSetting = ResourceLoader.Load<SettingData>(_userSettingPath);
		}
	}
	public SettingData GetUserSetting()
	{
		_userSetting = ResourceLoader.Load<SettingData>(_userSettingPath);
		return _userSetting;
	}
	public void SaveUserSetting(SettingData runtimeSetting)
	{
		
		ResourceSaver.Save(runtimeSetting, _userSettingPath);
		
	}

	public SettingData GetDefaultSetting()
	{
		return _defaultSetting;
	}
	
}
