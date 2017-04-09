using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScenarioViewer : MonoBehaviour {
    [SerializeField]
    Text _text;

    [SerializeField]
    Image _faceImage;

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

        string script = _currentSceneMaster.texts[_textIndex];
        _textIndex++;
        ParseScript(script);
    }

    void Finish() {
        gameObject.SetActive(false);
    }

    void ParseScript(string script) {
        if (script.IndexOf("/") == 0) {
            Queue<string> arguments = new Queue<string>(script.Split(' '));
            string commandName = arguments.Dequeue();
            ParseCommnad(commandName, arguments.ToArray());
            return;
        }
        _text.text = script;
    }

    void ParseCommnad(string name, string[] arguments) {
        switch (name) {
            case "/face":
                var faceCode = uint.Parse(arguments[0]);
                var faceMaster = Master.Instance.FindFaceMasterBy(faceCode);
                _faceImage.sprite = faceMaster.CreateImageSprite();
                break;
        }
        Next();
    }
}
