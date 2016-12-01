using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public const float SPEED = 4f;
    public const int JUMP_POWER = 700;

    [SerializeField]
    private LayerMask _groundLayer;

    [SerializeField]
    private GameObject _bullet;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private Camera _camera;
    private bool _isGrounded;


    // Use this for initialization
    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        // LinecastでPlayerの足元に地面があるか判定
        _isGrounded = Physics2D.Linecast(
            transform.position + transform.up * 1,
            transform.position - transform.up * 0.05f,
            _groundLayer
        );

        // スペースキーを押し
        if (Input.GetKeyDown("space")) {
            // 着地してたとき
            if (_isGrounded) {
                // runアニメーションを止めて、jumpアニメーションを実行
                _animator.SetBool("run", false);
                _animator.SetTrigger("jump");
                // 着地判定をfalse
                _isGrounded = false;
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

        // 弾を打つ
        if (Input.GetKeyDown("left ctrl")) {
            _animator.SetTrigger("shot");
            Instantiate(_bullet, transform.position + new Vector3(0f, 1.2f, 0f), transform.rotation);
        }
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
            // カメラをプレイヤーに追従させる
            moveCamera();
        } else {
            // 横移動の速度を0にしてピタッと止まるようにする
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            // 待機
            _animator.SetBool("run", false);
        }
    }

    private void moveCamera() {
        const int THRESHOLD = 4;

        // 画面中央から左に少し移動した位置をユニティちゃんが超えたら
        if (transform.position.x > _camera.transform.position.x - THRESHOLD) {
            // カメラの位置を取得
            Vector3 cameraPosition = _camera.transform.position;
            // Playerの位置から右に少し移動した位置を画面中央にする
            cameraPosition.x = transform.position.x + THRESHOLD;
            _camera.transform.position = cameraPosition;
        }

        //カメラ表示領域の左下をワールド座標に変換
        Vector2 min = _camera.ViewportToWorldPoint(new Vector2(0, 0));
        //カメラ表示領域の右上をワールド座標に変換
        Vector2 max = _camera.ViewportToWorldPoint(new Vector2(1, 1));

        Vector2 playerPosition = transform.position;
        // Playerのx座標の移動範囲をClampメソッドで制限
        playerPosition.x = Mathf.Clamp(playerPosition.x, min.x + 0.5f, max.x);
        transform.position = playerPosition;
    }
}
