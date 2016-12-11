using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stage {
    public class Scene : MonoBehaviour {
        [SerializeField]
        GameObject _lifeGauge;

        [SerializeField]
        GameObject _gameOverLabel;

        Image _lifeGaugeImage;
        bool _gameOvered;

        // Use this for initialization
        void Start() {
            _lifeGaugeImage = _lifeGauge.GetComponent<Image>();
        }

        // Update is called once per frame
        void Update() {
            if (_lifeGaugeImage.fillAmount <= 0) {
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
