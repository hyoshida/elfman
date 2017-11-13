using UnityEngine;
using Assets.Scripts.Extensions;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerClingAction))]
public class PlayerJumpingAction : MonoBehaviour {
    public const float TIME_FOR_FULL_JUMP = 0.25f;

    [SerializeField]
    int _jumpPower;

    Player _player;
    PlayerClingAction _playerClingAction;
    PhysicsObject _physicsObject;
    Animator _animator;
    float _timeHeld;

    void Start() {
        _player = gameObject.GetComponent<Player>();
        _playerClingAction = gameObject.GetComponent<PlayerClingAction>();
        _animator = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate() {
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
        if (_playerClingAction.IsCling) {
            _playerClingAction.IsCling = false;
        } else {
            if (!_player.IsGrounded) {
                return false;
            }

            if (!_animator.IsPlaying("waiting", "running", "running-attack1", "dashing", "jumping5")) {
                return false;
            }
        }

        // runアニメーションを止めて、jumpアニメーションを実行
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger("jump");

        // AddForceにて上方向へ力を加える
        _player.velocity.y += _jumpPower * Time.deltaTime;

        return true;
    }

    void takenGravity() {
        if (_player.IsGrounded) {
            return;
        }
        float ratio = Mathf.SmoothStep(1f, 0f, _timeHeld / TIME_FOR_FULL_JUMP);
        _player.velocity.y += (_jumpPower * ratio * -0.75f) * Time.deltaTime;
    }
}
