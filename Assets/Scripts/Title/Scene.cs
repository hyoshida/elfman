using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Scene : MonoBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void onClickStartButton() {
        SceneManager.LoadScene("elfman");
    }

    public void onClickExitButton() {
        Application.Quit();
    }
}
