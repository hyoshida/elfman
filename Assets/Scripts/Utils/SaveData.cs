using UnityEngine;

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
    }

    public uint stageCode;
    public Vector2 playerPosition;
}