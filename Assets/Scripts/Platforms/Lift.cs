using UnityEngine;

public class Lift : MonoBehaviour {
    [Range(0f, 1f)]
    [SerializeField]
    float _amount;

    [SerializeField]
    float _degree;

    [SerializeField]
    float _distance;

    [SerializeField]
    float _speed = 1;

    Rigidbody2D _rigidbody2D;
    Vector3 _defaultPosition;

    GameObject _player;

    // Use this for initialization
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = DegreeToVector2(_degree) * _speed;
        _defaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        float distance = Vector3.Distance(_defaultPosition, transform.position);
        if (distance > _distance) {
            transform.position = (Vector2)_defaultPosition + (_rigidbody2D.velocity / _speed * _distance);

            _defaultPosition = transform.position;

            _rigidbody2D.velocity *= -1;
        }

        if (_player != null) {
            _player.transform.position += (Vector3)(_rigidbody2D.velocity * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject player = collision.gameObject;
        if (_player == null && player.tag == "Player") {
            _player = player;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        GameObject player = collision.gameObject;
        if (_player != null && player.tag == "Player") {
            _player = null;
        }
    }

    // TODO: この辺はUtil系のクラスにまとめたい
    Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    Vector2 DegreeToVector2(float degree) {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}