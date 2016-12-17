using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Still {
    public class Scene : MonoBehaviour {
        [SerializeField]
        GameObject _textArea;

        StillMaster _stillMaster;
        int _textIndex;
        Text _text;

        // Use this for initialization
        void Start() {
            _textIndex = 0;

            _text = _textArea.GetComponent<Text>();
        }

        void Awake() {
            StartCoroutine(LoadMaster());
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetButtonDown("Fire1")) {
                NextText();
            }
        }

        IEnumerator LoadMaster() {
            yield return Master.Load();

            uint stageCode = GameManager.Instance.CurrentStageCode;
            _stillMaster = Master.Instance.FindStillMasterBy(stageCode);
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