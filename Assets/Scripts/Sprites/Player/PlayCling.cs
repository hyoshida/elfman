using UnityEngine;
using Assets.Scripts.Utils;

// プレイヤーの壁張り付きまわりを実現する
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayCling : MonoBehaviour {
    Animator _animator;
    Rigidbody2D _rigidbody2D;
    bool _isCling;
    int _collidDirection; // none=0, left=-1, right=1

    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update() {
        MovePlayer();
        UpdatePlayerPosition();
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.contacts.Length <= 0) {
            return;
        }

        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            HitType hitType = util.HitTest();
            if ((hitType & HitType.WALL) != 0) {
                // PoligonColider2Dを使ってるせいかまともに機能しない
                // _collidDirection = ((hitType & HitType.LEFT) != 0) ? -1 : 1;

                const float playerWidth = 0.6f;
                int playerDirection = ((transform.localScale.x >= 0) ? 1 : -1);

                Vector3 center = transform.position + new Vector3(playerWidth * 0.5f * playerDirection, 0);
                Vector3 contactPoint = collision.contacts[0].point;

                bool right = contactPoint.x > center.x;
                _collidDirection = right ? 1 : -1;
            } else {
                _collidDirection = 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (_isCling) {
            return;
        }

        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            HitType hitType = util.HitTest();
            if ((hitType & HitType.WALL) == 0) {
                _collidDirection = 0;
            }
        }
    }

    void MovePlayer() {
        float axis = Input.GetAxisRaw("Horizontal");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction != 0) {
            _isCling = (_collidDirection == direction);
        } else {
            _isCling = false;
        }

        _animator.SetBool("isCling", _isCling);
    }

    void UpdatePlayerPosition() {
        if (_isCling) {
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
        } else {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}