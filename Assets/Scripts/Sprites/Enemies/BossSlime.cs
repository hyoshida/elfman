using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BossSlime : Slime {
    [SerializeField]
    public int maxDivisionHp;

    Renderer _renderer;
    int _divisionHp;

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
}