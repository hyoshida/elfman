using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class PlayerCamera : MonoBehaviour {
    const float CAMERA_OFFSET_Y = 0.5f;
    const float THRESHOLD_TOP = -4.5f;
    const float THRESHOLD_BOTTOM = 4.5f;

    Camera _camera;
    Renderer _renderer;
    float _targetCameraPositionY;
    bool _shouldUpdateImmediately;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.battleMode == BattleMode.Stage) {
            MoveCameraX();
            UpdateCameraY();
            MoveCameraY();
        }
    }

    void MoveCameraX() {
        // Playerの位置から右に少し移動した位置を画面中央にする
        Vector3 cameraPosition = _camera.transform.position;
        cameraPosition.x = transform.position.x;
        _camera.transform.position = cameraPosition;
    }

    void MoveCameraY() {
        if (float.IsNaN(_targetCameraPositionY)) {
            return;
        }

        // 画面からプレイヤーが見切れてるときは即座にカメラを移動する
        if (_shouldUpdateImmediately) {
            _camera.transform.DOKill();

            Vector3 cameraPosition = _camera.transform.position;
            cameraPosition.y = _targetCameraPositionY;
            _camera.transform.position = cameraPosition;

            _targetCameraPositionY = float.NaN;
            return;
        }

        float during = 1.5f;
        _camera.transform.DOMoveY(_targetCameraPositionY, during).OnComplete(() => _targetCameraPositionY = float.NaN);
    }

    void UpdateCameraY() {
        Vector3 cameraPosition = _camera.transform.position;

        cameraPosition.y = transform.position.y + CAMERA_OFFSET_Y;

        var playerHeight = _renderer.bounds.size.y;
        Vector2 min = _camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = _camera.ViewportToWorldPoint(new Vector2(1, 1));
        bool moveToTop = ((transform.position.y + (playerHeight / 2)) > max.y);
        bool moveToBottom = (transform.position.y - (playerHeight / 2) < min.y);
        if (moveToTop || moveToBottom) {
            // プレイヤーが見切れていたらカメラを即移動させる
            _shouldUpdateImmediately = true;

            if (moveToTop) {
                cameraPosition.y = transform.position.y + THRESHOLD_TOP;
            } else {
                cameraPosition.y = transform.position.y + THRESHOLD_BOTTOM;
            }
        } else {
            _shouldUpdateImmediately = false;
        }

        // TODO: 場所によってカメラの移動限界を変えたい・・・。やっぱりカメラコリジョン必要？
        if (cameraPosition.y < 0) {
            cameraPosition.y = 0;
        }

        // Y座標は MoveCameraY 関数によって緩やかに移動させる
        _targetCameraPositionY = cameraPosition.y;
    }
}