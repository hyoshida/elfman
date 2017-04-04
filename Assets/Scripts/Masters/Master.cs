using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class Master {
    private static Master _instance;
    private static Master _filePath;

    public static IEnumerator Load() {
        if (_instance == null) {
            string path = Path.Combine(Application.streamingAssetsPath, "Masters/master.json");
            if (path.Contains("://")) {
                WWW www = new WWW(path);
                yield return www;
                _instance = JsonUtility.FromJson<Master>(www.text);
            } else {
                var json = File.ReadAllText(path);
                _instance = JsonUtility.FromJson<Master>(json);
            }
        }

        foreach (var stillMaster in _instance.stillMasters) {
            yield return stillMaster.Load();
        }
    }

    public static Master Instance {
        get {
            return _instance;
        }
    }

    public List<StillMaster> stillMasters = new List<StillMaster> { };
    public List<SceneMaster> sceneMasters = new List<SceneMaster> { };

    public StillMaster FindStillMasterBy(uint code, bool boss) {
        foreach (var stillMaster in stillMasters) {
            if (stillMaster.code == code && stillMaster.boss == boss) {
                return stillMaster;
            }
        }
        return null;
    }

    public SceneMaster FindSceneMasterBy(uint stageCode, bool opening) {
        foreach (var sceneMaster in sceneMasters) {
            if (sceneMaster.stageCode == stageCode && sceneMaster.opening == opening) {
                return sceneMaster;
            }
        }
        return null;
    }
}