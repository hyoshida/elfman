using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Renderer))]
public class Enemy : MonoBehaviour {
    [SerializeField]
    int hp;

    [SerializeField]
    Material _flashMaterial;

    Renderer _renderer;
    Material _defaultMaterial;

    bool IsDead {
        get {
            return (hp <= 0);
        }
    }

    void Start() {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.material;
    }

    void Damage(int amount = 1) {
        hp -= amount;
        StartCoroutine(DamageAndInvinciblePhase());
    }

    IEnumerator DamageAndInvinciblePhase() {
        // 10回点滅
        for (int i = 0; i < 10; i++) {
            // 真っ白にする
            _renderer.material = _flashMaterial;
            // 0.05秒待つ
            yield return new WaitForSeconds(0.05f);
            // 元に戻す
            _renderer.material = _defaultMaterial;
            // 0.05秒待つ
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Bullet") {
            Destroy(collider.gameObject);
            Damage();
        }

        if (collider.tag == "Sword") {
            PlayerSword sowrd = collider.gameObject.GetComponent<PlayerSword>();
            Damage(sowrd.Strength);
        }

        if (IsDead) {
            Destroy(gameObject);
        }
    }
}
