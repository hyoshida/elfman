using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerVO {
    static PlayerVO _instance;

    public static PlayerVO Instance {
        get {
            if (_instance == null) {
                _instance = new PlayerVO();
            }
            return _instance;
        }
    }

    public List<uint> clearedStageCodes;

    public void Load() {
        LoadClearedStageCodes();
    }

    public void Save() {
        SaveClearedStageCodes();
        PlayerPrefs.Save();
    }

    public void ClearStage(uint stageCode) {
        clearedStageCodes.Add(stageCode);
        Save();
    }

    void LoadClearedStageCodes() {
        string clearedStageCodesString = PlayerPrefs.GetString("clearedStageCodes", "");

        clearedStageCodes = new List<uint> { };

        foreach (string code in clearedStageCodesString.Split(',')) {
            try {
                clearedStageCodes.Add(uint.Parse(code));
            } catch (System.FormatException _error) {
                // エラーは無視する
            }
        }
    }

    void SaveClearedStageCodes() {
        var array = clearedStageCodes.Select((code) => code.ToString()).Distinct().ToArray();
        string clearedStageCodesString = string.Join(",", array);
        PlayerPrefs.SetString("clearedStageCodes", clearedStageCodesString);
    }
}