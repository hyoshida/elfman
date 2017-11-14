using Assets.Scripts.Utils;
using Assets.Scripts.Extensions;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Slime : MonoBehaviour {
    enum AIState {
        Idle,
        Wait,
        Waiting,
        Walk,
        Walking,
        Attack,
        Attacking,
    }

    public const int SPEED = -6;
    public readonly float GROUND_ANGLE_TOLERANCE = Mathf.Cos(30.0f * Mathf.Deg2Rad);

    AIState _aiState;
    AIState _prevAiState;
    Enemy _enemy;
    Animator _animator;

    // Use this for initialization
    void Start() {
        _aiState = AIState.Idle;
        _enemy = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (_enemy.frozen) {
            return;
        }
        UpdateForAI();
    }

    // TODO: 下記の形式で書き直す
    // var count = rigibody2d.Cast(movement, contactFilter2d, hitBuffer, distance + shellRadius);
    void OnCollisionEnter2D(Collision2D collision) {
        CollisionUtil util = new CollisionUtil(collision);
        if (!util.IsLayer("Ground")) {
            return;
        }

        HitType hitType = util.HitTest();
        if ((hitType & HitType.WALL) != 0) {
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
                    _enemy.movementX = SPEED * direction;
                } else {
                    _enemy.movementX = 0;
                    _prevAiState = AIState.Walking;
                    _aiState = AIState.Idle;
                }
                break;
        }
    }
}