using UnityEngine;
using Assets.Scripts.Extensions;

[RequireComponent(typeof(Player))]
public class PlayerJumpingAction : MonoBehaviour {
    public const float TIME_FOR_FULL_JUMP = 0.25f;

    [SerializeField]
    int _jumpPower;

    Player _player;
    Rigidbody2D _rigidbody2D;
    Animator _animator;
    float _timeHeld;

    void Start() {
        _player = gameObject.GetComponent<Player>();
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
    }

    void Update() {
        if (_player.IsFrozen) {
            return;
        }
        Action();
    }


    void Action() {
        if (Input.GetButtonDown("Jump")) {
            bool jumped = Jump();
            if (jumped) {
                _timeHeld = 0f;
            }
        }

        if (Input.GetButton("Jump")) {
            _timeHeld += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") || _timeHeld >= TIME_FOR_FULL_JUMP) {
            takenGravity();
            _timeHeld = TIME_FOR_FULL_JUMP;
        }
    }

    bool Jump() {
        if (_animator.GetBool("isCling")) {
            _animator.SetBool("isCling", false);
        } else {
            if (!_player.IsGrounded) {
                return false;
            }

            if (!_animator.IsPlaying("waiting", "running", "running-attack1", "dashing", "jumping5")) {
                return false;
            }
        }

        _player.IsGrounded = false;

        // runアニメーションを止めて、jumpアニメーションを実行
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger("jump");

        // AddForceにて上方向へ力を加える
        _rigidbody2D.AddForce(Vector2.up * _jumpPower);

        return true;
    }

    void takenGravity() {
        if (_player.IsGrounded) {
            return;
        }
        float ratio = Mathf.SmoothStep(1f, 0f, _timeHeld / TIME_FOR_FULL_JUMP);
        _rigidbody2D.AddRelativeForce(Vector2.up * _jumpPower * ratio * -0.75f);
    }
}
