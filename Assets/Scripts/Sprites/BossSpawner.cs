using UnityEngine;

public class BossSpawner : MonoBehaviour {
    [SerializeField]
    GameObject _enemyPrefab;

    GameObject _enemy;
    bool _spawned;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (_spawned) {
            watchEnemy();
        }
    }

    public void Spawn() {
        if (_spawned) {
            return;
        }
        _spawned = true;

        _enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);
        _enemy.transform.SetParent(transform.parent);
    }

    void watchEnemy() {
        if (_enemy) {
            return;
        }

        // TODO: この辺の作りかなり雑
        var stageScene = ApplicationScene.Instance as Stage.Scene;
        if (stageScene) {
            stageScene.GameOver();
        }
    }
}