using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour {
    [SerializeField]
    GameObject _goal;

    [SerializeField]
    GameObject _bossSpawner;

    GameObject _player;
    bool _closed;

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
        yield return new WaitForSeconds(0.5f);

        _player.transform.position = _goal.transform.position;

        yield return new WaitForSeconds(0.5f);

        _bossSpawner.GetComponent<BossSpawner>().Spawn();

        yield return new WaitForSeconds(1f);

        GameManager.Instance.gameState = GameState.Playing;
        GameManager.Instance.battleMode = BattleMode.Boss;
    }
}
