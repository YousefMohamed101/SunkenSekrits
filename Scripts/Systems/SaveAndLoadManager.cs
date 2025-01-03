using Godot;
using System;

public partial class SaveAndLoadManager : Node
{
	public static SaveAndLoadManager Instance;
	private SettingData DefaultSetting = GD.Load<SettingData>("res://Data/SettingsData/DefaultSetting.tres");
	private SettingData UserSetting;
	private string sUserSettingPath= "user://Data/UserSetting.tres";
	private string sDataPath= "user://Data";
	public override void _Ready()
	{
		InitializeUserData();
		Instance = this;
	}


	private void InitializeUserData()
	{
		
		if (DirAccess.DirExistsAbsolute(sDataPath) != true)
		{
			DirAccess.MakeDirAbsolute(sDataPath);
		}

		if (FileAccess.FileExists(sUserSettingPath) != true)
		{
			UserSetting = DefaultSetting;
			UserSetting.sID = "UserSetting";
			ResourceSaver.Save(UserSetting, sUserSettingPath);
		}
		else
		{
			UserSetting = ResourceLoader.Load<SettingData>(sUserSettingPath);
		}
	}
	public SettingData GetUserSetting()
	{
		UserSetting = ResourceLoader.Load<SettingData>(sUserSettingPath);
		return UserSetting;
	}
	public void SaveUserSetting(SettingData RuntimeSetting)
	{
		
		ResourceSaver.Save(RuntimeSetting, sUserSettingPath);
		
	}

	public SettingData GetDefaultSetting()
	{
		return DefaultSetting;
	}
	
}
