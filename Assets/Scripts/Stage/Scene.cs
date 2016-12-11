using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    public class Scene : MonoBehaviour {
        [SerializeField]
        GameObject _player;

        [SerializeField]
        GameObject _gameOverLabel;

        Player _playerInstance;
        bool _gameOvered;

        // Use this for initialization
        void Start() {
            _playerInstance = _player.GetComponent<Player>();
        }

        // Update is called once per frame
        void Update() {
            if (_playerInstance.IsDead) {
                GameOver();
            }
        }

        public void GameOver() {
            if (_gameOvered) {
                return;
            }
            _gameOvered = true;
            _gameOverLabel.SetActive(true);
        }
    }
}
