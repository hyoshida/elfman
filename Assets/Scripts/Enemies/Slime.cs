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
    public const float WAIT_SECONDS = 0.5f;

    AIState _aiState;
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
                _aiState = (Random.value >= 0.5f) ? AIState.Walk : AIState.Wait;
                _waitingTime = 0;
                break;
            case AIState.Wait:
                _waitingTime += Time.deltaTime;
                if (_waitingTime >= WAIT_SECONDS) {
                    _aiState = AIState.Idle;
                }
                break;
            case AIState.Walk:
                _aiState = AIState.Walking;
                _animator.SetTrigger("walk");
                _rigidbody2D.velocity = new Vector2(SPEED, _rigidbody2D.velocity.y);
                break;
            case AIState.Walking:
                if (!_animator.GetBool("walk")) {
                    _rigidbody2D.velocity = new Vector2(-0, _rigidbody2D.velocity.y);
                    _aiState = AIState.Idle;
                }
                break;
        }
    }
}