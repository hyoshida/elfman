using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    enum State {
        None,
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
        SceneMaster _currentSceneMaster;

        private State state {
            get {
                return _state;
            }

            set {
                if (_state == value) {
                    return;
                }

                _state = value;

                switch (_state) {
                    case State.Scenario:
                        OnChangeStateToScenario();
                        break;
                }
            }
        }

        public void GameStart() {
            if (state == State.Playing) {
                return;
            }
            state = State.Playing;

            _hud.SetActive(true);
            _scenarioViewer.SetActive(false);

            StartCoroutine(PlayAfterCurrentFrame());
        }

        public void GameOver() {
            if (state == State.GameOver) {
                return;
            }
            state = State.GameOver;
            _gameOverLabel.SetActive(true);
            _playerInstance.Dispose();
        }

        public void GameClear() {
            if (state == State.GameClear) {
                return;
            }
            state = State.GameClear;
            _gameClearLabel.SetActive(true);
        }

        // Use this for initialization
        void Start() {
            state = State.Scenario;
            _playerInstance = _player.GetComponent<Player>();

            _hud.SetActive(false);

            GameManager.Instance.gameState = GameState.Pause;
        }

        // Update is called once per frame
        void Update() {
            switch (state) {
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

        void OnChangeStateToScenario() {
            _scenarioViewer.SetActive(true);

            // TODO: ちゃんとエンディングを再生できるようにする
            bool openning = true;
            _currentSceneMaster = Master.Instance.FindSceneMasterBy(GameManager.Instance.CurrentStageCode, openning);
        }

        void UpdateForScenarioState() {
            // TODO: マスタを参照する
            if (_currentSceneMaster == null) {
                GameStart();
                return;
            }

            if (!Input.GetButtonDown("Fire1")) {
                return;
            }

            // TODO: シナリオを読む処理を実装する
            bool finished = true;
            if (finished) {
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
