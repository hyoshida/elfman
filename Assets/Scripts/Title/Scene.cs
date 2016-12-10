using UnityEngine;

public class Scene : MonoBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void onClickStartButton() {
        GameManager.Instance.SwitchScene(GameScene.elfman);
    }

    public void onClickExitButton() {
        GameManager.Instance.Quit();
    }
}