using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    enum State {
        Scenario,
        Playing,
        GameOver,
        GameClear,
    }

    public class Scene : ApplicationScene {
        [SerializeField]
        GameObject _player;

        [SerializeField]
        GameObject _hud;

        [SerializeField]
        GameObject _gameOverLabel;

        [SerializeField]
        GameObject _gameClearLabel;

        [SerializeField]
        GameObject _scenarioViewer;

        Player _playerInstance;
        bool _gameOvered;
        State _state;

        public void GameStart() {
            if (_state == State.Playing) {
                return;
            }
            _state = State.Playing;

            _hud.SetActive(true);
            _scenarioViewer.SetActive(false);

            StartCoroutine(PlayAfterCurrentFrame());
        }

        public void GameOver() {
            if (_state == State.GameOver) {
                return;
            }
            _state = State.GameOver;
            _gameOverLabel.SetActive(true);
            _playerInstance.Dispose();
        }

        public void GameClear() {
            if (_state == State.GameClear) {
                return;
            }
            _state = State.GameClear;
            _gameClearLabel.SetActive(true);
        }

        // Use this for initialization
        void Start() {
            _state = State.Scenario;
            _playerInstance = _player.GetComponent<Player>();

            _hud.SetActive(false);
            _scenarioViewer.SetActive(true);

            GameManager.Instance.gameState = GameState.Pause;
        }

        // Update is called once per frame
        void Update() {
            switch (_state) {
                case State.Scenario:
                    UpdateForScenarioState();
                    break;
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

        void UpdateForScenarioState() {
            // TODO: マスタを参照する

            if (Input.GetButtonDown("Fire1")) {
                GameStart();
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

        // NOTE: 決定キー後に攻撃モーションをとるのを避けるために１フレームだけポーズ解除をズラす
        IEnumerator PlayAfterCurrentFrame() {
            yield return new WaitForEndOfFrame();
            GameManager.Instance.gameState = GameState.Playing;
        }
    }
}
