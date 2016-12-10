using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    Title,
    elfman
}

public class GameManager {
    public static readonly ReadOnlyCollection<string> SCENES = Array.AsReadOnly(new string[] {
        "Title",
        "elfman"
	});

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

    public void SwitchScene(GameScene scene) {
        SceneManager.LoadScene(scene.ToString());
    }

    public void Quit() {
        Application.Quit();
    }
}