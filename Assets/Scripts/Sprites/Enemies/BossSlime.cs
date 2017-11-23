using Assets.Scripts.Extensions;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BossSlime : MonoBehaviour {
    enum AIState {
        Idle,
        Wait,
        Waiting,
        Walk,
        Walking,
        Attack,
        Attacking,
    }

    public const int SPEED = -3;
    public readonly float GROUND_ANGLE_TOLERANCE = Mathf.Cos(30.0f * Mathf.Deg2Rad);

    [SerializeField]
    public int maxDivisionHp;

    AIState _aiState;
    AIState _prevAiState;
    Enemy _enemy;
    Animator _animator;
    Rigidbody2D _rigibody2d;
    ContactFilter2D _contactFilter2d;
    Renderer _renderer;
    int _divisionHp;

    float direction {
        get { return gameObject.transform.localScale.x; }
    }

    // Use this for initialization
    void Start() {
        _aiState = AIState.Idle;
        _enemy = GetComponent<Enemy>();
        _enemy.OnDamage += OnDamage;
        _animator = GetComponent<Animator>();
        _rigibody2d = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<Renderer>();

        _contactFilter2d.useTriggers = false;
        _contactFilter2d.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter2d.useLayerMask = true;

        _divisionHp = maxDivisionHp;
    }

    // Update is called once per frame
    void Update() {
        if (_enemy.IsFrozen) {
            return;
        }
        UpdateForAI();
    }

    void FixedUpdate() {
        if (_enemy.frozen) {
            return;
        }

        var movement = new Vector2(SPEED * direction, 0);
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

            Vector2 scale = gameObject.transform.localScale;
            scale.x *= -1;
            gameObject.transform.localScale = scale;
            return;
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
                    _enemy.movementX = SPEED * direction;
                } else {
                    _enemy.movementX = 0;
                    _prevAiState = AIState.Walking;
                    _aiState = AIState.Idle;
                }
                break;
        }
    }

    void OnDamage(int amount) {
        if (_enemy.IsDead) {
            return;
        }

        _divisionHp -= amount;
        if (_divisionHp <= 0) {
            _divisionHp = maxDivisionHp;
            Divide();
        }
    }

    void Divide() {
        Debug.Log("Divide");
        var height = _renderer.bounds.size.y / transform.localScale.y;
        transform.localScale *= 0.75f;
        transform.position += (Vector3.up * height * 0.5f);
        Instantiate(gameObject);
    }
}