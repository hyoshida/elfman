using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerSword : MonoBehaviour {
    public bool Boost;

    [SerializeField]
    public int _strength;

    public int Strength {
        get { return _strength * (Boost ? 2 : 1); }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Enemy") {
            // NOTE: 攻撃ヒット中はゲーム進行をスローにしたい
            Time.timeScale = 0.5f;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        Time.timeScale = 1;
    }

    void OnDisable() {
        Time.timeScale = 1;
    }
}
