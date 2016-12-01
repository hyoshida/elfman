using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public const int SPEED = 10;

    private GameObject _player;

    void Start() {
        _player = GameObject.FindWithTag("Player");

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        // Playerの向いている方向に弾を飛ばす
        rigidbody2D.velocity = new Vector2(SPEED * _player.transform.localScale.x, rigidbody2D.velocity.y);

        // 画像の向きをPlayerに合わせる
        Vector2 scale = transform.localScale;
        scale.x = _player.transform.localScale.x;
        transform.localScale = scale;

        // 5秒後に消滅
        Destroy(gameObject, 5);
    }
}
