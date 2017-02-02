using UnityEngine;
using Assets.Scripts.Extensions;

public class PlayerJumpingAction {
    public const float TIME_FOR_FULL_JUMP = 0.25f;

    Player _player;
    int _jumpPower;
    Rigidbody2D _rigidbody2D;
    Animator _animator;
    float _timeHeld;

    public PlayerJumpingAction(Player player, int jumpPower) {
        _player = player;
        _jumpPower = jumpPower;
        _rigidbody2D = player.gameObject.GetComponent<Rigidbody2D>();
        _animator = player.gameObject.GetComponent<Animator>();
    }

    public void Update() {
        if (Input.GetButtonDown("Jump")) {
            _timeHeld = 0f;
            Jump();
        }

        if (Input.GetButton("Jump")) {
            _timeHeld += Time.deltaTime;
        }

        if ((Input.GetButtonUp("Jump") || _timeHeld >= TIME_FOR_FULL_JUMP) && !_player.IsGrounded) {
            float ratio = Mathf.SmoothStep(1f, 0f, _timeHeld / TIME_FOR_FULL_JUMP);
            _rigidbody2D.AddRelativeForce(Vector2.up * _jumpPower * ratio * -0.75f);
            _timeHeld = TIME_FOR_FULL_JUMP;
        }
    }

    void Jump() {
        if (!_player.IsGrounded) {
            return;
        }

        if (!_animator.IsPlaying("waiting", "running", "running-attack1", "dashing", "jumping5")) {
            return;
        }

        _player.IsGrounded = false;

        // runアニメーションを止めて、jumpアニメーションを実行
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger("jump");

        // AddForceにて上方向へ力を加える
        _rigidbody2D.AddForce(Vector2.up * _jumpPower);
    }
}
