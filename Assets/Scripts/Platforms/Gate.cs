using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour {
    [SerializeField]
    GameObject _goal;

    GameObject _player;
    bool _closed;

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

        GameManager.Instance.gameState = GameState.Playing;
        GameManager.Instance.battleMode = BattleMode.Boss;
    }
}
