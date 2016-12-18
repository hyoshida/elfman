using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    Title,
    Collection,
    Still,
    Stage1
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

    GameScene _currentScene;
    uint _currentStageCode;

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

    public void GotoStage(uint stageCode) {
        // TODO: ステージ選択できるようにする
        SwitchScene(GameScene.Stage1);
        _currentStageCode = stageCode;
    }

    void SwitchScene(GameScene scene) {
        FadeManager.Instance.Run(() =>
            SceneManager.LoadScene(scene.ToString())
        );
        _currentScene = scene;
    }
}