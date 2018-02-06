using Stage;
using UnityEngine;

public class Zenny : MonoBehaviour {
    public const float FOLLOWING_SPEED = 50f;

    [SerializeField]
    int _amount;

    GameObject _player;
    float _deltaTime;
    Rigidbody2D _rigidbody2D;
    bool _followed;

    // Use this for initialization
    void Start() {
        var currentScene = Camera.main.GetComponent<Scene>();
        _player = currentScene.Player;

        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        _deltaTime += Time.deltaTime;
        if (_deltaTime >= 2.0f) {
            if (!_followed) {
                _rigidbody2D.isKinematic = true;
            }
            _followed = true;

            FollowingPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "Player") {
            return;
        }

        Destroy(gameObject);

        var player = collider.gameObject.GetComponent<Player>();
        if (player != null) {
            player.TakeZenny(_amount);
        }
    }

    void FollowingPlayer() {
        transform.LookAt(_player.transform);
        var offset = transform.forward * FOLLOWING_SPEED * Time.deltaTime;
        transform.position += new Vector3(offset.x, offset.y);
    }
}