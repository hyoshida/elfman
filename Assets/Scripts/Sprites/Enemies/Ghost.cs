using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Ghost : MonoBehaviour {
    public const int SPEED = -3;

    Enemy _enemy;
    Rigidbody2D _rigidbody2D;

    // Use this for initialization
    void Start() {
        _enemy = GetComponent<Enemy>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (_enemy.frozen) {
            return;
        }
        _rigidbody2D.velocity = new Vector2(SPEED, _rigidbody2D.velocity.y);
    }
}
