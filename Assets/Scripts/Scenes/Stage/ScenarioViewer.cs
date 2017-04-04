using UnityEngine;
using UnityEngine.UI;

public class ScenarioViewer : MonoBehaviour {
    [SerializeField]
    Text _text;

    SceneMaster _currentSceneMaster;
    int _textIndex;

    void OnEnable() {
        _textIndex = 0;

        // TODO: ちゃんとエンディングを再生できるようにする
        bool openning = true;
        _currentSceneMaster = Master.Instance.FindSceneMasterBy(GameManager.Instance.CurrentStageCode, openning);

        Next();
    }

    void Update() {
        if (_currentSceneMaster == null) {
            Finish();
            return;
        }

        if (Input.GetButtonDown("Fire1")) {
            Next();
        }
    }

    void Next() {
        if (_currentSceneMaster == null) {
            Finish();
            return;
        }

        bool finished = (_textIndex >= _currentSceneMaster.texts.Count);
        if (finished) {
            Finish();
            return;
        }

        string text = _currentSceneMaster.texts[_textIndex];
        _text.text = text;

        _textIndex++;
    }

    void Finish() {
        gameObject.SetActive(false);
    }
}
