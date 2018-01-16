using Assets.Scripts.Extensions;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BossSlime : Slime {
    [SerializeField]
    public int maxDivisionHp;

    [SerializeField]
    GameObject _slimBall;

    Renderer _renderer;
    int _divisionHp;
    float _totalDaltaTime;
    float _lastDaltaTime;

    float direction {
        get { return gameObject.transform.localScale.x; }
    }

    protected new void Start() {
        base.Start();
        _enemy.OnDamage += OnDamage;
        _renderer = GetComponent<Renderer>();
        _divisionHp = maxDivisionHp;
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
        var height = _renderer.bounds.size.y / transform.localScale.y;
        transform.localScale *= 0.75f;
        transform.position += (Vector3.up * height * 0.5f);
        Instantiate(gameObject);
    }

    override protected void UpdateForAI() {
        _totalDaltaTime += Time.deltaTime;
        var deltaTime = _totalDaltaTime - _lastDaltaTime;

        switch (_aiState) {
            case AIState.Idle:
                _aiState = (_prevAiState == AIState.Waiting) ? AIState.Attack : AIState.Wait;
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
            case AIState.Attack:
                _aiState = AIState.Attacking;
                _lastDaltaTime = _totalDaltaTime;
                LanchSlimBall();
                break;
            case AIState.Attacking:
                if (deltaTime <= 5f) {
                    break;
                }
                _prevAiState = AIState.Attacking;
                _aiState = AIState.Idle;
                break;
        }
    }

    void LanchSlimBall() {
        var height = _renderer.bounds.size.y / transform.localScale.y;
        var positionOffset = (Vector3.up * height);

        var slimBallGameObject = Instantiate(_slimBall, transform.position + positionOffset, transform.rotation);
        var slimBallInstance = slimBallGameObject.GetComponent<SlimBall>();
        slimBallInstance.Initialize(gameObject);
    }
}