using Assets.Scripts.Extensions;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Slime : MonoBehaviour {
    public readonly float GROUND_ANGLE_TOLERANCE = Mathf.Cos(30.0f * Mathf.Deg2Rad);

    protected enum AIState {
        Idle,
        Wait,
        Waiting,
        Walk,
        Walking,
        Attack,
        Attacking,
    }

    [SerializeField]
    int speed = 6;

    protected AIState _aiState;
    protected AIState _prevAiState;
    protected Enemy _enemy;
    protected Animator _animator;
    protected Rigidbody2D _rigibody2d;
    protected ContactFilter2D _contactFilter2d;

    float direction {
        get { return gameObject.transform.localScale.x; }
    }

    // Use this for initialization
    protected void Start() {
        _aiState = AIState.Idle;
        _enemy = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();
        _rigibody2d = GetComponent<Rigidbody2D>();

        _contactFilter2d.useTriggers = false;
        _contactFilter2d.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter2d.useLayerMask = true;
    }

    // Update is called once per frame
    protected void Update() {
        if (_enemy.IsFrozen) {
            return;
        }
        UpdateForAI();
    }

    protected void FixedUpdate() {
        if (_enemy.frozen) {
            return;
        }

        var movement = new Vector2(-speed * direction, 0);
        var distance = movement.x;
        if (Mathf.Abs(distance) <= 0.01f) {
            return;
        }

        int groundLayer = LayerMask.NameToLayer("Ground");
        var raycastHit2d = new RaycastHit2D[16];
        var count = _rigibody2d.Cast(movement, _contactFilter2d, raycastHit2d, 0.1f);

        for (var i = 0; i < count; i++) {
            if (raycastHit2d[i].collider.gameObject.layer != groundLayer) {
                continue;
            }

            var currentNomal = raycastHit2d[i].normal;
            if (currentNomal.y != 0) {
                // 直角な壁でないなら
                continue;
            }

            var scale = gameObject.transform.localScale;
            scale.x *= -1;
            gameObject.transform.localScale = scale;
            return;
        }
    }

    protected void UpdateForAI() {
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
                    _enemy.movementX = -speed * direction;
                } else {
                    _enemy.movementX = 0;
                    _prevAiState = AIState.Walking;
                    _aiState = AIState.Idle;
                }
                break;
        }
    }
}