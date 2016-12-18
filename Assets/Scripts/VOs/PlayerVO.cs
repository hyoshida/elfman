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

    public List<uint> unlockedStillCodes;

    public void Load() {
        LoadUnlockedStillCodes();
    }

    public void Save() {
        SaveUnlockedStillCodes();
        PlayerPrefs.Save();
    }

    public void UnlockStill(uint stillCode) {
        unlockedStillCodes.Add(stillCode);
        Save();
    }

    void LoadUnlockedStillCodes() {
        string unlockedStillCodesString = PlayerPrefs.GetString("unlockedStillCodes", "");

        unlockedStillCodes = new List<uint> { };

        foreach (string code in unlockedStillCodesString.Split(',')) {
            try {
                unlockedStillCodes.Add(uint.Parse(code));
            } catch (System.FormatException _error) {
                // エラーは無視する
            }
        }
    }

    void SaveUnlockedStillCodes() {
        var array = unlockedStillCodes.Select((code) => code.ToString()).Distinct().ToArray();
        string unlockedStillCodesString = string.Join(",", array);
        PlayerPrefs.SetString("unlockedStillCodes", unlockedStillCodesString);
    }
}