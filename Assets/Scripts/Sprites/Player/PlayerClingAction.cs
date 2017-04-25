using UnityEngine;
using Assets.Scripts.Utils;

// プレイヤーの壁張り付きまわりを実現する
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerClingAction : MonoBehaviour {
    // 0.25秒間は再度張り付きできない
    const int CLING_RECAST_MSEC = 250;

    Animator _animator;
    Rigidbody2D _rigidbody2D;
    int _collidDirection; // none=0, left=-1, right=1
    float _leftRecastMsec;

    public bool IsCling {
        get {
            return _animator.GetBool("isCling");
        }

        set {
            bool wasCling = IsCling;
            if (wasCling == value) {
                return;
            }

            if (wasCling) {
                _leftRecastMsec = CLING_RECAST_MSEC;
            }

            _animator.SetBool("isCling", value);

            ChangeRigibodyType();
        }
    }

    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update() {
        MovePlayer();
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
        if (IsCling) {
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
        if (!IsCling) {
            _leftRecastMsec -= Time.deltaTime * 1000;
            if (_leftRecastMsec > 0) {
                // リキャスト中は張り付けない
                return;
            }
        }

        float axis = Input.GetAxisRaw("Horizontal");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction != 0) {
            IsCling = (_collidDirection == direction);
        } else {
            IsCling = false;
        }
    }

    void ChangeRigibodyType() {
        if (IsCling) {
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
        } else {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}