using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : PhysicsObject {
    [SerializeField]
    public int hp;

    [SerializeField]
    public int power;

    [SerializeField]
    Material _flashMaterial;

    [SerializeField]
    GameObject _boodParticlePrefab;

    [SerializeField]
    GameObject _zennyPrefab;

    [SerializeField]
    int _zennyAmount;

    Renderer _renderer;
    Material _defaultMaterial;
    CameraShaker _cameraShaker;
    bool _isFrozen;

    public float movementX {
        set {
            targetVelocity.x = value;
        }
    }

    public bool IsDead {
        get {
            return (hp <= 0);
        }
    }

    public bool IsFrozen {
        get {
            return _isFrozen;
        }
    }

    float Height {
        get { return _renderer.bounds.size.y / transform.localScale.y; }
    }

    public void Stop() {
        targetVelocity = new Vector2(0, 0);
    }

    protected override void ComputeVelocity() {
        if (frozen) {
            return;
        }

        // todo...
    }

    void Awake() {
        _renderer = GetComponent<Renderer>();
        _defaultMaterial = _renderer.material;

        Camera camera = Camera.main;
        _cameraShaker = camera.GetComponent<CameraShaker>();

        GameManager.Instance.gameState.watcher += OnChangeGameState;
    }

    void OnDestroy() {
        GameManager.Instance.gameState.watcher -= OnChangeGameState;
    }

    void OnChangeGameState(GameState newState, GameState oldState) {
        if (newState == GameState.Pause) {
            Stop();
            _isFrozen = true;
            return;
        }
        _isFrozen = false;
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
        Vector3 offset = Vector3.up * Height * 0.5f;
        GameObject bloodParticle = (GameObject)Instantiate(_boodParticlePrefab, transform.position + offset, Quaternion.identity);

        const float timeToSucied = 1.5f;
        Destroy(bloodParticle, timeToSucied);
    }

    void Die() {
        Vector3 offset = Vector3.up * Height * 0.5f;
        for (var i = 0; i < _zennyAmount; i++) {
            var gameObject = Instantiate(_zennyPrefab, transform.position + offset, Quaternion.identity);
            var rigidbody2d = gameObject.GetComponent<Rigidbody2D>();

            float angle = Random.Range(0f, Mathf.PI * 2);
            var x = Mathf.Sin(angle);
            var y = Mathf.Cos(angle);
            var forward = new Vector2(x, y);

            rigidbody2d.AddForce(forward * 200f);
        }

        Destroy(gameObject);
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
            Die();
        }
    }
}
