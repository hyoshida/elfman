using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Renderer))]
public class Enemy : MonoBehaviour {
    [SerializeField]
    int hp;

    [SerializeField]
    Material _flashMaterial;

    [SerializeField]
    GameObject _boodParticlePrefab;

    Renderer _renderer;
    Material _defaultMaterial;
    CameraShaker _cameraShaker;

    bool IsDead {
        get {
            return (hp <= 0);
        }
    }

    void Start() {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.material;

        Camera camera = Camera.main;
        _cameraShaker = camera.GetComponent<CameraShaker>();
    }

    void OnDestroy() {
    }

    void Damage(int amount = 1) {
        hp -= amount;
        StartCoroutine(DamageAndInvinciblePhase());
        ShakeCamera();
        PlayBloodParticle();
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

    void ShakeCamera() {
        const float duration = 0.25f;
        const float strength = 0.25f;
        const int vibrato = 20;
        _cameraShaker.Shake(duration, Vector3.one * strength, vibrato);
    }

    void PlayBloodParticle() {
        Vector3 offset = Vector3.up * 1f;
        GameObject bloodParticle = (GameObject)Instantiate(_boodParticlePrefab, transform.position + offset, Quaternion.identity);

        const float timeToSucied = 1.5f;
        Destroy(bloodParticle, timeToSucied);
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
