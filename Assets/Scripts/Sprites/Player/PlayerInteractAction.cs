using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ↑ボタンを押すとプレイヤーの付近にあるものにインタラクトする
public class PlayerInteractAction : MonoBehaviour {
    void Update() {
        float axis = Input.GetAxisRaw("Vertical");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction == 1) {
            Interact();
        }
    }

    void Interact() {
        // TODO: 実装する
        Debug.Log("Interact!!");
    }
}
