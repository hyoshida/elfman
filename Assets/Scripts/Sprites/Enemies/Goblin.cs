using Assets.Scripts.Extensions;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Goblin : MonoBehaviour {
    enum AIState {
        Idle,
        Wait,
        Waiting,
        Walk,
        Walking,
        Attack,
        Attacking,
    }

    public const int SPEED = -4;
    public const float WALKING_DISTANCE = 10f;
    public const float ATTACKING_DISTANCE = 1.25f;

    [SerializeField]
    GameObject _target;

    AIState _aiState;
    AIState _prevAiState;
    Enemy _enemy;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    bool CanAttack {
        get {
            return Mathf.Abs(GetDistanceForTarget()) < ATTACKING_DISTANCE;
        }
    }

    bool CanWalk {
        get {
            return Mathf.Abs(GetDistanceForTarget()) < WALKING_DISTANCE;
        }
    }


    // Use this for initialization
    void Start() {
        _aiState = AIState.Idle;
        _enemy = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (_enemy.IsFrozen) {
            return;
        }
        UpdateForAI();
    }

    void UpdateForAI() {
        switch (_aiState) {
            case AIState.Idle:
                if (CanAttack) {
                    _aiState = AIState.Attack;
                } else if (CanWalk) {
                    _aiState = AIState.Walk;
                } else {
                    _aiState = AIState.Wait;
                }
                break;
            case AIState.Wait:
                _aiState = AIState.Waiting;
                break;
            case AIState.Waiting:
                if (CanAttack) {
                    _aiState = AIState.Attack;
                } else if (_animator.IsPlaying("Waiting")) {
                    _prevAiState = AIState.Waiting;
                    _aiState = AIState.Idle;
                }
                break;
            case AIState.Walk:
                _aiState = AIState.Walking;
                _animator.SetTrigger("walk");
                break;
            case AIState.Walking:
                if (CanAttack) {
                    Debug.Log("CanAttack");
                    _aiState = AIState.Attack;
                } else if (_animator.IsPlaying("Walking")) {
                    int direction = GetTargetDirection();
                    transform.localScale = new Vector2(direction, transform.localScale.y);
                    _rigidbody2D.velocity = new Vector2(SPEED * direction, _rigidbody2D.velocity.y);
                } else {
                    _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
                    _prevAiState = AIState.Walking;
                    _aiState = AIState.Idle;
                }
                break;
            case AIState.Attack:
                _aiState = AIState.Attacking;
                _animator.SetTrigger("attack");
                _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
                break;
            case AIState.Attacking:
                if (!_animator.IsPlaying("Attacking")) {
                    _prevAiState = AIState.Waiting;
                    _aiState = AIState.Idle;
                }
                break;
        }
    }

    float GetDistanceForTarget() {
        return transform.position.x - _target.transform.position.x;
    }

    int GetTargetDirection() {
        return (GetDistanceForTarget() > 0) ? 1 : -1;
    }
}