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
    public const int SPEED = -3;
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
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (collision.gameObject.layer == groundLayer) {
            // from http://www.gamedev.net/topic/673693-how-to-check-if-grounded-in-2d-unity-game/#entry5265297
            foreach (ContactPoint2D contact in collision.contacts) {
                if (Vector3.Dot(contact.normal, Vector3.up) > GROUND_ANGLE_TOLERANCE) {
                    // this collider is touching "ground"
                } else {
                    // this collider is touching "wall"
                    Vector2 scale = gameObject.transform.localScale;
                    scale.x *= -1;
                    gameObject.transform.localScale = scale;
                }
            }
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
                if (!AnimatorIsPlaying("slime-wait")) {
                    _prevAiState = AIState.Waiting;
                    _aiState = AIState.Idle;
                }
                break;
            case AIState.Walk:
                _aiState = AIState.Walking;
                _animator.SetTrigger("walk");
                break;
            case AIState.Walking:
                if (AnimatorIsPlaying("slime-walk")) {
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

    bool AnimatorIsPlaying(string stateName) {
        return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    bool AnimatorIsPlaying() {
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.length > animatorStateInfo.normalizedTime;
    }
}