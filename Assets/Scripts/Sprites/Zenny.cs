using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zenny : MonoBehaviour {
    [SerializeField]
    int _amount;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "Player") {
            return;
        }

        Destroy(gameObject);

        var player = collider.gameObject.GetComponent<Player>();
        if (player != null) {
            player.TakeZenny(_amount);
        }
    }
}