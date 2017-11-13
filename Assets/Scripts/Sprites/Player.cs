using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Extensions;
using System;
using DG.Tweening;

public class Player : PhysicsObject {
    public const float RUNNING_SPEED = 6f;
    public const float DASHING_SPEED = RUNNING_SPEED * 1.5f;
    public const int DASH_POWER = 7000;

    [SerializeField]
    LayerMask _groundLayer;

    [SerializeField]
    GameObject _bullet;

    [SerializeField]
    GameObject _lifeGauge;

    Animator _animator;
    Rigidbody2D _rigidbody2D;
    Camera _camera;
    SpriteRenderer _renderer;
    Image _lifeGaugeImage;
    GhostSprites _ghostSprites;
    int _lastRunningDirection;
    float _lastRunningAt;
    float _lastWaitingAt;
    bool _isFrozen;
    CameraShaker _cameraShaker;
    Vector3 _previousPosition;

    public float HpRatio {
        get {
            return _lifeGaugeImage.fillAmount;
        }
    }

    public bool IsDead {
        get {
            if (_lifeGaugeImage.fillAmount <= 0) {
                return true;
            }

            return false;
        }
    }

    public bool IsGrounded {
        get {
            return grounded;
        }
    }

    public bool IsFrozen {
        get {
            return _isFrozen;
        }
    }

    bool IsFalldowned {
        get {
            return (transform.position.y < _camera.transform.position.y - 10);
        }
    }
    bool IsAttacking {
        get {
            return _animator.IsPlaying("attacking1", "attacking2", "attacking3");
        }
    }

    float jumpTakeOffSpeed = 7;
    float maxSpeed = 8;

    protected override void ComputeVelocity() {
        var movement = Vector2.zero;

        movement.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp("Jump") && (velocity.y > 0)) {
            velocity.y = velocity.y * 0.5f;
        }

        targetVelocity = movement * maxSpeed;
    }

    // すべての動きを止める
    public void Stop() {
        _lastWaitingAt = Time.time;

        _animator.SetBool("isRunning", false);
        _animator.SetBool("isDashing", false);

        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
    }

    public void Dispose() {
        GameManager.Instance.gameState.watcher -= OnChangeGameState;

        Destroy(gameObject);

        // バイブ消し忘れを防止する
        Vibrate(false);
    }

    // Use this for initialization
    void Awake() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _cameraShaker = _camera.GetComponent<CameraShaker>();
        _renderer = GetComponent<SpriteRenderer>();
        _lifeGaugeImage = _lifeGauge.GetComponent<Image>();
        _ghostSprites = GetComponent<GhostSprites>();

        GameManager.Instance.gameState.watcher += OnChangeGameState;

        // ゲーム初期化時にポーズかどうかを確認しておく
        _isFrozen = (GameManager.Instance.GameState == GameState.Pause);
    }

    // Update is called once per frame
    // void Update() {
    //     if (IsFrozen) {
    //         return;
    //     }

    //     ActionPlayer();
    //     MovePlayer();

    //     // if (IsFalldowned) {
    //     //     PutBackPlayer();
    //     //     Damage();
    //     // }
    // }

    // void FixedUpdate() {
    // }

    void OnDestroy() {
        Dispose();
    }

    void OnChangeGameState(GameState newState, GameState oldState) {
        if (newState == GameState.Pause) {
            Stop();
            _isFrozen = true;
            return;
        }
        _isFrozen = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            Damage();
        }
    }

    void Damage() {
        _lifeGaugeImage.fillAmount -= 0.1f;
        ShakeCamera();
        StartCoroutine(DamageVibrate());
        StartCoroutine(DamageAndInvinciblePhase());
    }

    void ShakeCamera() {
        const float duration = 0.25f;
        const float strength = 0.25f;
        const int vibrato = 20;
        _cameraShaker.Shake(duration, Vector3.one * strength, vibrato);
    }

    IEnumerator DamageVibrate() {
        Vibrate(true);
        yield return new WaitForSeconds(0.25f);
        Vibrate(false);
    }

    void Vibrate(bool enabled) {
        const int playerIndex = 0;
        float motorPower = enabled ? 1.0f : 0.0f;
        XInputDotNetPure.GamePad.SetVibration(playerIndex, motorPower, motorPower);
    }

    void ActionPlayer() {
        // 上下への移動速度を取得
        float velY = _rigidbody2D.velocity.y;
        // 移動速度が0.1より大きければ上昇
        bool isJumping = (velY > 0.1f);
        // 移動速度が-0.1より小さければ下降
        bool isFalling = (velY < -0.1f);
        // アニメーションに反映する
        _animator.SetBool("isJumping", isJumping);
        _animator.SetBool("isFalling", isFalling);

        // 近距離攻撃
        if (Input.GetButtonDown("Fire1")) {
            if (IsGrounded) {
                bool isDashing = _animator.GetBool("isDashing");
                if (isDashing) {
                    StartCoroutine(DashAttackingPhase());
                } else {
                    _animator.SetBool("isDashing", false);
                    _animator.SetBool("isRunning", false);
                }
            }
            _animator.SetTrigger("attack");
        }

        // 遠距離攻撃
        if (Input.GetButtonDown("Fire2")) {
            _animator.SetTrigger("shot");
            Instantiate(_bullet, transform.position + new Vector3(0f, 0f, 0f), transform.rotation);
        }
    }

    void MovePlayer() {
        bool isDashing = _animator.GetBool("isDashing");

        // 左=-1、右=1
        float axis = Input.GetAxisRaw("Horizontal");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction != 0 && !IsAttacking) {
            // 方向キー２度押しでダッシュ開始
            float doubleTapTime = _lastWaitingAt - _lastRunningAt;
            if (((doubleTapTime > 0) && (doubleTapTime < 0.15f)) && (_lastRunningDirection == direction) && IsGrounded) {
                _animator.SetBool("isDashing", true);
                _ghostSprites.enabled = true;
                isDashing = true;
            }

            // Fire3ボタンでもダッシュ開始
            if (Input.GetButtonDown("Fire3") && IsGrounded) {
                _animator.SetBool("isDashing", true);
                _ghostSprites.enabled = true;
                isDashing = true;
            }

            _lastRunningAt = Time.time;
            _lastRunningDirection = direction;

            _animator.SetBool("isRunning", true);

            Vector2 scale = transform.localScale;
            scale.x = Math.Abs(scale.x) * direction;
            transform.localScale = scale;

            float speed = isDashing ? DASHING_SPEED : RUNNING_SPEED;
            float slopeFriction = calcSlopFriction();
            _rigidbody2D.velocity = new Vector2(direction * speed * (1f - slopeFriction), _rigidbody2D.velocity.y);
        } else {
            Stop();
        }

        if (!isDashing && !_ghostSprites.HasGhosts()) {
            // 立ち止まったら残像を消す
            _ghostSprites.enabled = false;
        }
    }

    float calcSlopFriction() {
        float slopeFriction = 0f;

        var playerSize = new Vector3(0, _renderer.bounds.size.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position - (playerSize / 2), -Vector2.up, 1f);

        if (hit.collider != null) {
            slopeFriction = 1f - Vector3.Dot(hit.normal, Vector3.up);
        }

        return slopeFriction;
    }

    IEnumerator DamageAndInvinciblePhase() {
        // レイヤーをInvincibleに変更
        gameObject.layer = LayerMask.NameToLayer("Invincible");

        // 10回点滅
        for (int i = 0; i < 10; i++) {
            // 透明にする
            _renderer.material.color = new Color(1, 1, 1, 0);
            // 0.05秒待つ
            yield return new WaitForSeconds(0.05f);
            // 元に戻す
            _renderer.material.color = new Color(1, 1, 1, 1);
            // 0.05秒待つ
            yield return new WaitForSeconds(0.05f);
        }

        // レイヤーをPlayerに戻す
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    IEnumerator DashAttackingPhase() {
        _isFrozen = true;

        yield return new WaitForEndOfFrame();
        _animator.SetBool("isDashing", false);
        _animator.SetBool("isRunning", false);

        const float SLIDING_SEC = 0.3f;

        float time = 0f;
        while (time < SLIDING_SEC) {
            yield return new WaitForEndOfFrame();

            float ratio = Mathf.SmoothStep(1f, 0f, time / SLIDING_SEC);
            float speed = DASHING_SPEED * ratio;
            _rigidbody2D.velocity = new Vector2(transform.localScale.x * speed, _rigidbody2D.velocity.y);

            time += Time.deltaTime;
        }

        _isFrozen = false;
    }

    void PutBackPlayer() {
        if (_previousPosition == Vector3.zero) {
            return;
        }
        transform.position = _previousPosition;
        _rigidbody2D.velocity = Vector2.zero;
    }
}
