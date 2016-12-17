using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    Title,
    Still,
    elfman
}

public class GameManager {
    public static readonly ReadOnlyCollection<string> SCENES = Array.AsReadOnly(new string[] {
        "Title",
        "elfman"
	});

    private static GameManager _instance;
    private static GameScene _currentScene;
    private static uint _currentStageCode;

    public static GameManager Instance {
        get {
            if (_instance != null) {
	            return _instance;
            }
            _instance = new GameManager();
            return _instance;
        }
    }

    public static GameScene CurrentScene {
        get {
            return _currentScene;
        }
    }

    public static uint CurrentStageCode {
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

    public void GotoStill() {
        SwitchScene(GameScene.Still);
    }

    public void GotoStage(uint stageCode) {
        SwitchScene(GameScene.elfman);
        _currentStageCode = stageCode;
    }

    void SwitchScene(GameScene scene) {
        FadeManager.Instance.Run(() =>
            SceneManager.LoadScene(scene.ToString())
        );
        _currentScene = scene;
    }
}