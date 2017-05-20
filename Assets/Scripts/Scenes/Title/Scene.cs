using UnityEngine;
using UnityEngine.UI;

namespace Title {
    public class Scene : ApplicationScene {
        [SerializeField]
        Button _defaultButton;

        // Use this for initialization
        void Start() {
            _defaultButton.Select();
        }

        public void onClickStartButton() {
            GameManager.Instance.GotoStage(1);
        }

        public void onClickCollectionButton() {
            GameManager.Instance.GotoCollection();
        }

        public void onClickExitButton() {
            GameManager.Instance.Quit();
        }
    }
}