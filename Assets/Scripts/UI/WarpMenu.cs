using Stage;
using UnityEngine;
using UnityEngine.UI;

public class WarpMenu : MonoBehaviour {
    [SerializeField]
    Button _defaultMenuItem;

    Scene _scene;

    Scene scene {
        get {
            return _scene = _scene ? _scene : (Scene)ApplicationScene.Instance;
        }
    }

    public void OnOpen() {
        scene.RevertFromMenuMode();

        gameObject.SetActive(true);
        scene.ChangeToMenuMode(() => gameObject.SetActive(false));

        _defaultMenuItem.Select();
    }

    public void OnSelectStage(string stageCode) {
        GameManager.Instance.GotoStage(uint.Parse(stageCode));
    }

    public void OnCancel() {
        scene.RevertFromMenuMode();
    }
}