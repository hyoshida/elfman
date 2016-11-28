using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public const float SPEED = 4f;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;

    // Use this for initialization
    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
    }

    void FixedUpdate() {
        // 左キー: -1、右キー: 1
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0) {
            // 入力方向へ移動
            _rigidbody2D.velocity = new Vector2(x * SPEED, _rigidbody2D.velocity.y);
            // localScale.xを-1にすると画像が反転する
            Vector2 temp = transform.localScale;
            temp.x = x;
            transform.localScale = temp;
            // 走る
            _animator.SetBool("run", true);
        } else {
            // 横移動の速度を0にしてピタッと止まるようにする
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            // 待機
            _animator.SetBool("run", false);
        }
    }
}