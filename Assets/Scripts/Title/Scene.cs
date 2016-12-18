using System.Collections;
using UnityEngine;

public class Scene : MonoBehaviour {
    // Use this for initialization
    void Start() {
        StartCoroutine(LoadMaster());
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Submit")) {
            onClickStartButton();
        }
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

    IEnumerator LoadMaster() {
        yield return Master.Load();
    }
}