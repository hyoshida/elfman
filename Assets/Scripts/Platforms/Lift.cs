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

    Vector3 _defaultPosition;
    Vector2 _velocity;
    GameObject _player;
    BoxCollider2D _boxCollider2d;

    // Use this for initialization
    void Start() {
        _velocity = DegreeToVector2(_degree) * _speed;
        _defaultPosition = transform.position;
        _boxCollider2d = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        transform.position += (Vector3)(_velocity * Time.deltaTime);

        float distance = Vector3.Distance(_defaultPosition, transform.position);
        if (distance > _distance) {
            _defaultPosition += (Vector3)(_velocity / _speed * _distance);
            _velocity *= -1;
        }

        if (_player != null) {
            _player.transform.position += (Vector3)(_velocity * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject player = collision.gameObject;
        if (_player == null && player.tag == "Player") {
            if (IsCarrying(collision)) {
                _player = player;
                Debug.Log("[Lift] Grab Player!");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        GameObject player = collision.gameObject;
        if (_player != null && player.tag == "Player") {
            _player = null;
            Debug.Log("[Lift] Ungrab Player!");
        }
    }

    bool IsCarrying(Collision2D collision) {
        GameObject target = collision.gameObject;

        Vector2 targetSize = collision.collider.bounds.size;
        Vector3 targetCenter = collision.collider.bounds.center;
        float targetBottom = targetCenter.y - (targetSize.y / 2f);

        Vector2 size = _boxCollider2d.size;
        Vector3 center = _boxCollider2d.bounds.center;
        float top = center.y + (size.y / 2f);

        if (Mathf.Abs(targetBottom - top) < 0.05f) {
            return true;
        }

        return false;
    }

    // TODO: この辺はUtil系のクラスにまとめたい
    Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    Vector2 DegreeToVector2(float degree) {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}