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
    public const int SPEED = -1;

    AIState _aiState;
    AIState _prevAiState;
    Animator _animator;
    Rigidbody2D _rigidbody2D;
    float _waitingTime;

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

    void UpdateForAI() {
        switch (_aiState) {
            case AIState.Idle:
                _aiState = (_prevAiState == AIState.Waiting) ? AIState.Walk : AIState.Wait;
                _waitingTime = 0;
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
                    _rigidbody2D.velocity = new Vector2(SPEED, _rigidbody2D.velocity.y);
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