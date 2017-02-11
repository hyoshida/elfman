using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Utils;
using Assets.Scripts.Extensions;
using System;

public class Player : MonoBehaviour {
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
    bool _isGrounded;
    Renderer _renderer;
    Image _lifeGaugeImage;
    GhostSprites _ghostSprites;
    int _lastRunningDirection;
    float _lastRunningAt;
    float _lastWaitingAt;
    bool _frozen;

    public bool IsDead {
        get {
            if (_lifeGaugeImage.fillAmount <= 0) {
                return true;
            }

            if (transform.position.y < _camera.transform.position.y - 10) {
                return true;
            }

            return false;
        }
    }

    public bool IsGrounded {
        get {
            return _isGrounded;
        }

        set {
            _isGrounded = value;
        }
    }

    public bool IsFrozen {
        get {
            return _frozen;
        }
    }

    bool IsAttacking {
        get {
            return _animator.IsPlaying("attacking1", "attacking2", "attacking3");
        }
    }

    // Use this for initialization
    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
        _lifeGaugeImage = _lifeGauge.GetComponent<Image>();
        _ghostSprites = GetComponent<GhostSprites>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.gameState == GameState.Pause) {
            return;
        }
        if (!_frozen) {
            ActionPlayer();
            MovePlayer();
        }
        MoveCamera();
    }

    void FixedUpdate() {
        // カメラをプレイヤーに追従させる
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            Damage();
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            HitType hitType = util.HitTest();
            if (hitType == HitType.GROUND) {
                _isGrounded = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            HitType hitType = util.HitTest();
            if (hitType == HitType.GROUND) {
                _isGrounded = false;
            }
        }
    }

    void Damage() {
        _lifeGaugeImage.fillAmount -= 0.1f;
        StartCoroutine(DamageAndInvinciblePhase());
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
            bool isDashing = _animator.GetBool("isDashing");
            if (isDashing) {
                StartCoroutine(DashAttackingPhase());
            } else {
                _animator.SetBool("isDashing", false);
                _animator.SetBool("isRunning", false);
            }
            _animator.SetTrigger("attack");
        }

        // 遠距離攻撃
        if (Input.GetButtonDown("Fire2")) {
            _animator.SetTrigger("shot");
            Instantiate(_bullet, transform.position + new Vector3(0f, 0f, 0f), transform.rotation);
        }

        // ダッシュ
        if (Input.GetButtonDown("Fire3")) {
            // _animator.SetTrigger("dash");
            // _rigidbody2D.AddForce(Vector2.right * DASH_POWER);
        }
    }

    void MoveCamera() {
        if (GameManager.Instance.battleMode != BattleMode.Stage) {
            return;
        }

        Vector3 cameraPosition = _camera.transform.position;

        // Playerの位置から右に少し移動した位置を画面中央にする
        cameraPosition.x = transform.position.x;

        const float THRESHOLD_TOP = 5f;
        const float THRESHOLD_BOTTOM = 2.15f;
        var playerHeight = _renderer.bounds.size.y;
        Vector2 min = _camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = _camera.ViewportToWorldPoint(new Vector2(1, 1));
        if ((transform.position.y - playerHeight) > max.y - THRESHOLD_TOP) {
            cameraPosition.y += (transform.position.y - playerHeight) - (max.y - THRESHOLD_TOP);
        } else if (transform.position.y < min.y + THRESHOLD_BOTTOM) {
            cameraPosition.y += transform.position.y - (min.y + THRESHOLD_BOTTOM);

            // TODO: 場所によってカメラの移動限界を変えたい・・・。やっぱりカメラコリジョン必要？
            if (cameraPosition.y < 0) {
                cameraPosition.y = 0;
            }
        }

        _camera.transform.position = cameraPosition;
    }

    void MovePlayer() {
        bool isDashing = _animator.GetBool("isDashing");

        // 左=-1、右=1
        float axis = Input.GetAxisRaw("Horizontal");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction != 0 && !IsAttacking) {
            float doubleTapTime = _lastWaitingAt - _lastRunningAt;
            if (((doubleTapTime > 0) && (doubleTapTime < 0.15f)) && (_lastRunningDirection == direction) && _isGrounded) {
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
            _rigidbody2D.velocity = new Vector2(transform.localScale.x * speed, _rigidbody2D.velocity.y);
        } else {
            _lastWaitingAt = Time.time;

            _animator.SetBool("isRunning", false);
            _animator.SetBool("isDashing", false);

            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        }

        if (!isDashing && !_ghostSprites.HasGhosts()) {
            // 立ち止まったら残像を消す
            _ghostSprites.enabled = false;
        }
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
        _frozen = true;

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

        _frozen = false;
    }
}
