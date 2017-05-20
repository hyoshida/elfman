using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Gate : MonoBehaviour {
    [SerializeField]
    GameObject _goal;

    [SerializeField]
    GameObject _bossSpawner;

    [SerializeField]
    [Range(0f, 10f)]
    float _during;

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
        GameManager.Instance.GameState = GameState.Pause;

        StartCoroutine(ForceMovePlayer());
    }

    IEnumerator ForceMovePlayer() {
        // プレイヤーの動きを止める
        Rigidbody2D rigidbody2D = _player.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(0, 0);

        // プレイヤーを自動的に横移動させる
        Vector3 position = new Vector3(_goal.transform.position.x, _player.transform.position.y, _player.transform.position.z);
        _player.transform.DOMove(position, _during).OnComplete(() => _playerMoved = true);

        // TODO: ここ雑だからなんとかしたい
        while (!_playerMoved) {
            yield return null;
        }

        _bossSpawner.GetComponent<BossSpawner>().Spawn();

        // TODO: プレイヤーを立ちモーションにしたい

        yield return new WaitForSeconds(1f);

        GameManager.Instance.GameState = GameState.Playing;
        GameManager.Instance.battleMode = BattleMode.Boss;
    }
}
