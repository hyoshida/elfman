using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    enum State {
        Playing,
        GameOver,
        GameClear,
    }

    public class Scene : ApplicationScene {
        [SerializeField]
        GameObject _player;

        [SerializeField]
        GameObject _gameOverLabel;

        [SerializeField]
        GameObject _gate;

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

        public void GameClear() {
            if (_state == State.GameClear) {
                return;
            }
            _state = State.GameClear;
            _gameOverLabel.SetActive(true);
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
                case State.GameClear:
                    UpdateForGameClearState();
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
                GameManager.Instance.GotoStill();
            }
        }

        void UpdateForGameClearState() {
            if (Input.GetButtonDown("Fire1")) {
                GameManager.Instance.GotoStageSelect();
            }
        }
    }
}
