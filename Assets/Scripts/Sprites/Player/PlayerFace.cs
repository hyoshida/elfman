using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFace : MonoBehaviour {
    [SerializeField]
    GameObject _playerFaceImage;

    [SerializeField]
    Sprite _normalFace;

    [SerializeField]
    Sprite _damagedFace;

    Image _image;
    Player _player;

    // Use this for initialization
    void Start() {
        _player = GetComponent<Player>();
        _image = _playerFaceImage.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (_player.HpRatio > 0.3) {
            _image.sprite = _normalFace;
        } else {
            _image.sprite = _damagedFace;
        }
    }
}
