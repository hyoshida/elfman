using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    enum State {
        Playing,
        GameOver,
    }

    public class Scene : MonoBehaviour {
        [SerializeField]
        GameObject _player;

        [SerializeField]
        GameObject _gameOverLabel;

        Player _playerInstance;
        bool _gameOvered;
        State _state;

        public void GameOver() {
            if (_state == State.GameOver) {
                return;
            }
            _state = State.GameOver;
            _gameOverLabel.SetActive(true);
            Destroy(_player);
        }

        // Use this for initialization
        void Start() {
            _state = State.Playing;
            _playerInstance = _player.GetComponent<Player>();
        }

        // Update is called once per frame
        void Update() {
            switch (_state) {
                case State.Playing:
                    UpdateForPlayingState();
                    break;
                case State.GameOver:
                    UpdateForGameOverState();
                    break;
            }
        }

        void UpdateForPlayingState() {
            if (_playerInstance.IsDead) {
                GameOver();
            }
        }

        void UpdateForGameOverState() {
            if (Input.GetButtonDown("Fire1")) {
                GameManager.Instance.SwitchScene(GameScene.Title);
            }
        }
    }
}
