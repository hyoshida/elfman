using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    Title,
    Collection,
    Still,
    StageSelect,
    Stage1
}

public enum GameState {
    Playing,
    Pause
}

public enum BattleMode {
    Stage,
    Boss
}

public class GameManager {
    private static GameManager _instance;

    public static GameManager Instance {
        get {
            if (_instance != null) {
	            return _instance;
            }
            _instance = new GameManager();
            return _instance;
        }
    }

    public BattleMode battleMode;
    public NotificationObject<GameState> gameState;

    GameScene _currentScene;
    uint _currentStageCode;

    public GameState GameState {
        get {
            return gameState.Value;
        }
        set {
            gameState.Value = value;
        }
    }

    public GameScene CurrentScene {
        get {
            return _currentScene;
        }
    }

    public uint CurrentStageCode {
        get {
            return _currentStageCode;
        }
    }

    public GameManager() {
        Application.targetFrameRate = 30;

        gameState = new NotificationObject<GameState>(GameState.Playing);
    }

    public void Quit() {
        Application.Quit();
    }

    public void GotoTitle() {
        SwitchScene(GameScene.Title);
    }

    public void GotoCollection() {
        SwitchScene(GameScene.Collection);
    }

    public void GotoStill() {
        SwitchScene(GameScene.Still);
    }

    public void GotoStageSelect() {
        SwitchScene(GameScene.StageSelect);
    }

    public void GotoStage(uint stageCode, Action callback = null) {
        battleMode = BattleMode.Stage;

        // TODO: ステージ選択できるようにする
        SwitchScene(GameScene.Stage1, callback);
        _currentStageCode = stageCode;
    }

    void SwitchScene(GameScene scene, Action callback = null) {
        FadeManager.Instance.Run(() => {
            if (callback != null) {
                SceneManager.sceneLoaded += (Scene _sceneName, LoadSceneMode _sceneMode) => callback();
            }
            SceneManager.LoadScene(scene.ToString());
        });
        _currentScene = scene;
    }
}