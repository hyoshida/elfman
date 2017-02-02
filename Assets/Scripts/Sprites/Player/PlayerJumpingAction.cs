using UnityEngine;
using Assets.Scripts.Extensions;

public class PlayerJumpingAction {
    Player _player;
    int _jumpPower;
    Rigidbody2D _rigidbody2D;
    Animator _animator;

    public PlayerJumpingAction(Player player, int jumpPower) {
        _player = player;
        _jumpPower = jumpPower;
        _rigidbody2D = player.gameObject.GetComponent<Rigidbody2D>();
        _animator = player.gameObject.GetComponent<Animator>();
    }

    public void Update() {
        if (Input.GetButtonDown("Jump")) {
            // 着地してたとき
            if (_player.IsGrounded && _animator.IsPlaying("waiting", "running", "running-attack1", "dashing", "jumping5")) {
                _player.IsGrounded = false;

                // runアニメーションを止めて、jumpアニメーションを実行
                _animator.SetBool("isRunning", false);
                _animator.SetTrigger("jump");

                // AddForceにて上方向へ力を加える
                _rigidbody2D.AddForce(Vector2.up * _jumpPower);
            }
        }
    }
}
