using Godot;
using Godot.Collections;

public partial class MainMenu : Control {
	private Button _newGameButton;
	private Button _loadGameButton;
	private Button _saveGameButton;
	private Button _settingsButton;
	private Button _quitGameButton;
	private Settings _settingsInitialize;
	private CenterContainer _menuHubCenterContainer;
	private AudioStreamPlayer _uisfxStreamPlayer;
	private int _selectedMenu;
	private Array<Node> _menuCheck;

	public override void _Ready() {
		_newGameButton = GetNode<Button>("%NewGame");
		_loadGameButton = GetNode<Button>("%LoadGame");
		_saveGameButton = GetNode<Button>("%SaveGame");
		_settingsButton = GetNode<Button>("%SettingsButton");
		_quitGameButton = GetNode<Button>("%QuitGame");
		_menuHubCenterContainer = GetNode<CenterContainer>("%MenuHub");
		_uisfxStreamPlayer = GetNode<AudioStreamPlayer>("%UISFX");

		_menuCheck = this.GetChildren();
		foreach(var component in _menuCheck) {
			if(component is Settings) {
				_settingsInitialize = GetNode<Settings>("%Settings");
				_settingsInitialize.SettingReturn += GetSettingsInitializeButtonHandling;
				_settingsButton.Pressed += GetSettingsInitializeButtonHandling;
				_settingsButton.Visible = true;
			}
		}

		_newGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_loadGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_saveGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_settingsButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_quitGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_newGameButton.Pressed += () => SceneTransitionManager.Instance.TransitionToScene("MainLevel");
		_quitGameButton.Pressed += () => GetTree().Quit();
	}

	private void GetSettingsInitializeButtonHandling() {
		if(_selectedMenu == 0) {
			_menuHubCenterContainer.Visible = false;
			_settingsInitialize.Open();
			_selectedMenu = 1;
		} else if(_selectedMenu == 1) {
			_menuHubCenterContainer.Visible = true;
			_settingsInitialize.Close();
			_selectedMenu = 0;
		}
	}
}