using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioViewer : MonoBehaviour {
    SceneMaster _currentSceneMaster;
    int _textIndex;

    void Finish() {
        gameObject.SetActive(false);
    }

    void OnEnable() {
        _textIndex = 0;
        // TODO: ちゃんとエンディングを再生できるようにする
        bool openning = true;
        _currentSceneMaster = Master.Instance.FindSceneMasterBy(GameManager.Instance.CurrentStageCode, openning);
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (_currentSceneMaster == null) {
            Finish();
            return;
        }

        if (!Input.GetButtonDown("Fire1")) {
            return;
        }

        bool finished = (_textIndex >= _currentSceneMaster.texts.Count);
        if (finished) {
            Finish();
            return;
        }

        string text = _currentSceneMaster.texts[_textIndex];
        // TODO: 画面に表示する
        Debug.Log(text);

        _textIndex++;
    }
}
