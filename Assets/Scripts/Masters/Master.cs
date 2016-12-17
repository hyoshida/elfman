using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class Master {
    private static Master _instance;

    public static Master Instance {
        get {
            if (_instance != null) {
	            return _instance;
            }
            string json = File.ReadAllText("Assets/Masters/master.json");
            _instance = JsonUtility.FromJson<Master>(json);
            return _instance;
        }
    }

    public List<StillMaster> stillMasters = new List<StillMaster> { };

    public StillMaster FindStillMasterBy(uint stageCode) {
        foreach (var stillMaster in stillMasters) {
            if (stillMaster.stageCode == stageCode) {
                return stillMaster;
            }
        }
        return null;
    }
}