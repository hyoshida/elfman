using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Gate : MonoBehaviour {
    [SerializeField]
    GameObject _goal;

    [SerializeField]
    GameObject _bossSpawner;

    GameObject _player;
    bool _closed;
    bool _playerMoved;

    void Start() {
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "Player") {
            return;
        }

        if (_closed) {
            return;
        }
        _closed = true;

        _player = collider.gameObject;
        GameManager.Instance.gameState = GameState.Pause;

        StartCoroutine(ForceMovePlayer());
    }

    IEnumerator ForceMovePlayer() {
        float during = 1.5f;
        Vector3 position = new Vector3(_goal.transform.position.x, _player.transform.position.y, _player.transform.position.z);
        _player.transform.DOMove(position, during).OnComplete(() => _playerMoved = true);

        // TODO: ここ雑だからなんとかしたい
        while (!_playerMoved) {
            yield return null;
        }

        _bossSpawner.GetComponent<BossSpawner>().Spawn();

        // TODO: プレイヤーを立ちモーションにしたい

        yield return new WaitForSeconds(1f);

        GameManager.Instance.gameState = GameState.Playing;
        GameManager.Instance.battleMode = BattleMode.Boss;
    }
}
