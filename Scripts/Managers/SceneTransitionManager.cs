using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransitionManager : Node {
	public static SceneTransitionManager Instance;

	private enum LoadingStates {
		Idle, Loading, Loaded, Error
	}


	private ColorRect _colorRect;
	private CanvasLayer _canvasLayer;
	private AnimationPlayer _animPlayer;
	private LoadingStates _currentLoadingState = LoadingStates.Idle;
	private PackedScene _loadedScene;
	private LevelData _levelData;

	public override void _Ready() {
		PersistantLoad();
		Instance = this;
	}

	public async void TransitionToScene(string sceneId) {
		_colorRect.Visible = true;
		_levelData = DataBaseManager.Instance.GetLevel(sceneId);
		if(_levelData == null) {
			GD.PrintErr("Scene " + sceneId + " not found");
			return;
		}

		_animPlayer.Play("FadeOut");
		await ToSignal(_animPlayer, "animation_finished");

		bool bLoadCheck = await LoadScene(_levelData.ScenePath);

		if(bLoadCheck && _loadedScene != null) {
			GetTree().ChangeSceneToPacked(_loadedScene);
			_animPlayer.Play("FadeIn");
			_colorRect.Visible = false;
		} else {
			_animPlayer.Play("FadeIn");
			GD.PrintErr("Failed to change the Scene to ", $"{sceneId}");
			_colorRect.Visible = false;
		}
	}

	private Task<bool> LoadScene(string scenePath) {
		_currentLoadingState = LoadingStates.Loading;
		GD.Print("Loading " + scenePath);
		try {
			ResourceLoader.LoadThreadedRequest(scenePath);
			while(_currentLoadingState == LoadingStates.Loading) {
				var status = ResourceLoader.LoadThreadedGetStatus(scenePath);

				if(status == ResourceLoader.ThreadLoadStatus.Loaded) {
					_currentLoadingState = LoadingStates.Loaded;
					_loadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(scenePath);

					return Task.FromResult(true);
				}

				if(status == ResourceLoader.ThreadLoadStatus.Failed) {
					_currentLoadingState = LoadingStates.Error;
					GD.PrintErr("Error loading scene: " + scenePath);
					return Task.FromResult(false);
				}

				if(status == ResourceLoader.ThreadLoadStatus.InvalidResource) {
					_currentLoadingState = LoadingStates.Error;
					GD.PrintErr("Invalid Resorce at: " + scenePath);
					return Task.FromResult(false);
				}

				if(status == ResourceLoader.ThreadLoadStatus.InProgress) {
					_currentLoadingState = LoadingStates.Loading;
					//GD.PrintErr("Resource still in process at" + sScenePath);
					//await Task.Delay(10);
				}
			}
		} catch(Exception e) {
			GD.PrintErr($"Exception during scene loading: {e.Message}");
			_currentLoadingState = LoadingStates.Error;
			return Task.FromResult(false);
		}

		return Task.FromResult(false);
	}


	private void PersistantLoad() {
		_canvasLayer = GetNode<CanvasLayer>("%CurtainHolder");
		_canvasLayer.Layer = 100;
		_colorRect = GetNode<ColorRect>("%CurtainTransition");
		_colorRect.Modulate = new Color(Colors.Black, 0);
		_colorRect.Visible = false;
		_animPlayer = GetNode<AnimationPlayer>("%TranstionAnimationManager");
	}
}