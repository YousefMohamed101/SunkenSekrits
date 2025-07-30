using Godot;
using Godot.Collections;

public partial class MainMenu : Control {
	private Button _loadGameButton;
	private Array<Node> _menuCheck;
	private CenterContainer _menuHubCenterContainer;
	private Button _newGameButton;
	private Button _quitGameButton;
	private Button _saveGameButton;
	private int _selectedMenu;
	private Button _settingsButton;
	private Settings _settingsInitialize;
	private AudioStreamPlayer _uisfxStreamPlayer;

	public override void _Ready() {
		_newGameButton = GetNode<Button>("%NewGame");
		_loadGameButton = GetNode<Button>("%LoadGame");
		_saveGameButton = GetNode<Button>("%SaveGame");
		_settingsButton = GetNode<Button>("%SettingsButton");
		_quitGameButton = GetNode<Button>("%QuitGame");
		_menuHubCenterContainer = GetNode<CenterContainer>("%MenuHub");
		_uisfxStreamPlayer = GetNode<AudioStreamPlayer>("%UISFX");

		_menuCheck = GetChildren();
		foreach(var component in _menuCheck) {
			if(component is Settings) {
				_settingsInitialize = GetNode<Settings>("%Settings");
				_settingsInitialize.SettingReturn += GetSettingsInitializeButtonHandling;
				_settingsButton.Pressed += GetSettingsInitializeButtonHandling;
				_settingsButton.Visible = true;
			}
		}

		if(GameManager.Instance.gamestate) {
			_saveGameButton.Show();
			_loadGameButton.Hide();
			_newGameButton.Hide();
		} else {
			_saveGameButton.Hide();
		}

		_newGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_loadGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_saveGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_settingsButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_quitGameButton.MouseEntered += () => _uisfxStreamPlayer.Play();
		_newGameButton.Pressed += NewGame;
		_quitGameButton.Pressed += () => GetTree().Quit();
		GameManager.Instance.GameOutofFocus += () => Show();
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

	private void NewGame() {
		Hide();
		GameManager.Instance.gamestate = true;
		SceneTransitionManager.Instance.TransitionToScene("MainLevel");
	}

	public override void _UnhandledInput(InputEvent @event) {
		if(Input.IsActionPressed("pause_game_ignore") && GameManager.Instance.gamepaused) {
			if(Visible) {
				Hide();
				Input.MouseMode = Input.MouseModeEnum.Captured;
				GameManager.Instance.gamepaused = false;
				GetTree().Paused = false;
				AcceptEvent();
			}
		}
	}
}