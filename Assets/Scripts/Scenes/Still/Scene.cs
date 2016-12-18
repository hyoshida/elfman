using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Still {
    public class Scene : ApplicationScene {
        [SerializeField]
        GameObject _textArea;

        [SerializeField]
        GameObject _image;

        StillMaster _stillMaster;
        int _textIndex;
        Text _text;

        // Use this for initialization
        void Start() {
            _textIndex = 0;

            _text = _textArea.GetComponent<Text>();

            uint stageCode = GameManager.Instance.CurrentStageCode;
            bool boss = false;
            _stillMaster = Master.Instance.FindStillMasterBy(stageCode, boss);

            Image image = _image.GetComponent<Image>();
            image.sprite = _stillMaster.createImageSprite();

            PlayerVO.Instance.UnlockStill(_stillMaster.code);
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetButtonDown("Fire1")) {
                NextText();
            }
        }

        void NextText() {
            List<string> texts = _stillMaster.texts;
            if (_textIndex >= texts.Count) {
                GameManager.Instance.GotoTitle();
                return;
            }

            _textArea.transform.parent.gameObject.SetActive(true);

            _text.text = texts[_textIndex];
            _textIndex++;
        }
    }
}