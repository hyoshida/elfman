using UnityEngine;

public class SlimBall : MonoBehaviour {
    public const int SPEED = 10;

    GameObject _parent;

    public void Initialize(GameObject parent) {
        _parent = parent;
        Lanch();
    }

    void Lanch() {
        var direction = (_parent.transform.localScale.x > 0) ? -1 : 1;

        var rigidbody2D = GetComponent<Rigidbody2D>();
        // 親の向いている方向に弾を飛ばす
        rigidbody2D.velocity = new Vector2(SPEED * direction, 0);

        // 画像の向きを親に合わせる
        var scale = transform.localScale;
        scale.x = direction;
        transform.localScale = scale;

        // 5秒後に消滅
        Destroy(gameObject, 5);
    }
}
