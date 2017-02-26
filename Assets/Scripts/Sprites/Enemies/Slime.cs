using Assets.Scripts.Utils;
using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AIState {
    Idle,
    Wait,
    Waiting,
    Walk,
    Walking,
    Attack,
    Attacking,
}

public class Slime : MonoBehaviour {
    public const int SPEED = -6;
    public readonly float GROUND_ANGLE_TOLERANCE = Mathf.Cos(30.0f * Mathf.Deg2Rad);

    AIState _aiState;
    AIState _prevAiState;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    // Use this for initialization
    void Start() {
        _aiState = AIState.Idle;
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        UpdateForAI();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        CollisionUtil util = new CollisionUtil(collision);
        if (!util.IsLayer("Ground")) {
            return;
        }

        HitType hitType = util.HitTest();
        if (hitType == HitType.WALL) {
            Vector2 scale = gameObject.transform.localScale;
            scale.x *= -1;
            gameObject.transform.localScale = scale;
        }
    }

    void UpdateForAI() {
        switch (_aiState) {
            case AIState.Idle:
                _aiState = (_prevAiState == AIState.Waiting) ? AIState.Walk : AIState.Wait;
                break;
            case AIState.Wait:
                _aiState = AIState.Waiting;
                break;
            case AIState.Waiting:
                if (!_animator.IsPlaying("slime-wait") || _animator.IsPlaying("Waiting")) {
                    _prevAiState = AIState.Waiting;
                    _aiState = AIState.Idle;
                }
                break;
            case AIState.Walk:
                _aiState = AIState.Walking;
                _animator.SetTrigger("walk");
                break;
            case AIState.Walking:
                if (_animator.IsPlaying("slime-walk") || _animator.IsPlaying("Walking")) {
                    float direction = gameObject.transform.localScale.x;
                    _rigidbody2D.velocity = new Vector2(SPEED * direction, _rigidbody2D.velocity.y);
                } else {
                    _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
                    _prevAiState = AIState.Walking;
                    _aiState = AIState.Idle;
                }
                break;
        }
    }
}