﻿using UnityEngine;
using System.Collections;

public class Scene : MonoBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void onClickStartButton() {
        Application.LoadLevel("elfman");
    }

    public void onClickExitButton() {
    }
}
