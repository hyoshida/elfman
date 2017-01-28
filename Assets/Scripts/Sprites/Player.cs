﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Utils;
using Assets.Scripts.Extensions;

public class Player : MonoBehaviour {
    public const float SPEED = 8f;
    public const int JUMP_POWER = 700;

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

    // Use this for initialization
    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
        _lifeGaugeImage = _lifeGauge.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.gameState == GameState.Pause) {
            return;
        }
        ActionPlayer();
        MovePlayer();
        MoveCamera();
    }

    void FixedUpdate() {
        // カメラをプレイヤーに追従させる
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            Damage();
        }

        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            HitType hitType = util.HitTest();
            if (hitType == HitType.GROUND) {
                _isGrounded = true;
            }
        }
    }

    void Damage() {
        _lifeGaugeImage.fillAmount -= 0.1f;
        StartCoroutine(DamageAndInvinciblePhase());
    }

    void ActionPlayer() {
        // ジャンプボタンを押し
        if (Input.GetButtonDown("Jump")) {
            // 着地してたとき
            if (_isGrounded && _animator.IsPlaying("waiting", "running", "running-attack1", "jumping5")) {
                _isGrounded = false;

                // runアニメーションを止めて、jumpアニメーションを実行
                _animator.SetBool("isRunning", false);
                _animator.SetTrigger("jump");

                // AddForceにて上方向へ力を加える
                _rigidbody2D.AddForce(Vector2.up * JUMP_POWER);
            }
        }

        // 上下への移動速度を取得
        float velY = _rigidbody2D.velocity.y;
        // 移動速度が0.1より大きければ上昇
        bool isJumping = (velY > 0.1f);
        // 移動速度が-0.1より小さければ下降
        bool isFalling = (velY < -0.1f);
        // アニメーションに反映する
        _animator.SetBool("isJumping", isJumping);
        _animator.SetBool("isFalling", isFalling);

        // 着地してなさそう
        if (isJumping || isFalling) {
            //_isGrounded = false;
        }

        // 近距離攻撃
        if (Input.GetButtonDown("Fire1")) {
            _animator.SetTrigger("attack");
        }

        // 遠距離攻撃
        if (Input.GetButtonDown("Fire2")) {
            _animator.SetTrigger("shot");
            Instantiate(_bullet, transform.position + new Vector3(0f, 0f, 0f), transform.rotation);
        }
    }

    void MoveCamera() {
        if (GameManager.Instance.battleMode != BattleMode.Stage) {
            return;
        }

        const float THRESHOLD_X = 4f;
        const float THRESHOLD_TOP = 5f;
        const float THRESHOLD_BOTTOM = 2.15f;

        Vector3 cameraPosition = _camera.transform.position;

        // Playerの位置から右に少し移動した位置を画面中央にする
        cameraPosition.x = transform.position.x + THRESHOLD_X;

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
        // 左=-1、右=1
        float axis = Input.GetAxisRaw("Horizontal");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction != 0) {
            _animator.SetBool("isRunning", true);

            Vector2 scale = transform.localScale;
            scale.x = direction;
            transform.localScale = scale;

            _rigidbody2D.velocity = new Vector2(transform.localScale.x * SPEED, _rigidbody2D.velocity.y);
        } else {
            _animator.SetBool("isRunning", false);

            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
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
}
