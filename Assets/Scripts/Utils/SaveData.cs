using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData {
    static string SAVE_KEY = "SaveData";
    static SaveData _instance;

    public static SaveData Instance {
        get {
            return _instance = _instance ?? new SaveData();
        }
    }

    static public void Save(GameObject player) {
        Instance.playerPosition = player.transform.position;
        Instance.stageCode = GameManager.Instance.CurrentStageCode;

        var json = JsonUtility.ToJson(Instance);
        PlayerPrefs.SetString(SAVE_KEY, json);
    }

    static public void Load() {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) {
            return;
        }

        var json = PlayerPrefs.GetString(SAVE_KEY);
        JsonUtility.FromJsonOverwrite(json, Instance);

        GameManager.Instance.GotoStage(Instance.stageCode, onGotoStage);
    }

    static void onGotoStage() {
        var player = FindPlayer();
        if (player != null) {
            player.transform.position = Instance.playerPosition;
        }
    }

    static GameObject FindPlayer() {
        var scene = SceneManager.GetActiveScene();
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        foreach (var gameObject in rootGameObjects) {
            if (gameObject.tag == "Player") {
                return gameObject;
            }
        }
        return null;
    }

    public uint stageCode;
    public Vector2 playerPosition;
}