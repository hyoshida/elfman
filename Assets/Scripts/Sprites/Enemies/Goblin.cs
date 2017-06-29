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
        LongRangeAttack,
        LongRangeAttacking,
    }

    public const int SPEED = -4;
    public const float LONG_RANGE_ATTACKING_DISTANCE = 10f;
    public const float WALKING_DISTANCE = 5f;
    public const float ATTACKING_DISTANCE = 1.25f;

    [SerializeField]
    GameObject _target;

    [SerializeField]
    GameObject _thorwablePrefab;

    AIState _aiState;
    AIState _prevAiState;
    Enemy _enemy;
    Animator _animator;
    Rigidbody2D _rigidbody2D;

    bool CanLongRangeAttack {
        get {
            return Mathf.Abs(GetDistanceForTarget()) < LONG_RANGE_ATTACKING_DISTANCE;
        }
    }

    bool CanWalk {
        get {
            return Mathf.Abs(GetDistanceForTarget()) < WALKING_DISTANCE;
        }
    }

    bool CanAttack {
        get {
            return Mathf.Abs(GetDistanceForTarget()) < ATTACKING_DISTANCE;
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
                } else if (CanLongRangeAttack) {
                    _aiState = AIState.LongRangeAttack;
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
                    // transform.rotation = Quaternion.LookRotation(_target.transform.position - transform.position);

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
            case AIState.LongRangeAttack:
                _aiState = AIState.LongRangeAttacking;
                _animator.SetTrigger("attack");
                _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
                ThrowToPlayer();
                break;
            case AIState.LongRangeAttacking:
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

    void ThrowToPlayer() {
        var thorwable = Instantiate(_thorwablePrefab, transform.position, transform.rotation);
        thorwable.transform.SetParent(transform.parent);

        const float firingAngle = 45.0f;
        const float gravity = 10.0f;

        // Calculate distance to target
        float distance = Vector2.Distance(transform.position, _target.transform.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float velocity = distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X/Y componenent of the velocity.
        float vx = Mathf.Sqrt(velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float vy = Mathf.Sqrt(velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Change the direction to face the target.
        vx *= -transform.localScale.x;

        var rigidbody2d = thorwable.GetComponent<Rigidbody2D>();
        rigidbody2d.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);
    }
}