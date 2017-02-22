using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class PlayerCamera : MonoBehaviour {
    const float THRESHOLD_TOP = 5f;
    const float THRESHOLD_BOTTOM = 3.5f;

    Camera _camera;
    Renderer _renderer;
    float _targetCameraPositionY;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.battleMode == BattleMode.Stage) {
            MoveCameraX();
            MoveCameraY();
            UpdateCameraY();
        }
    }

    void MoveCameraX() {
        // Playerの位置から右に少し移動した位置を画面中央にする
        Vector3 cameraPosition = _camera.transform.position;
        cameraPosition.x = transform.position.x;
        _camera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
    }

    void MoveCameraY() {
        if (float.IsNaN(_targetCameraPositionY)) {
            return;
        }
        float during = 1.5f;
        _camera.transform.DOMoveY(_targetCameraPositionY, during).OnComplete(() => _targetCameraPositionY = float.NaN);
    }

    void UpdateCameraY() {
        Vector3 cameraPosition = _camera.transform.position;

        var playerHeight = _renderer.bounds.size.y;
        Vector2 min = _camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = _camera.ViewportToWorldPoint(new Vector2(1, 1));
        if ((transform.position.y - playerHeight) > max.y - THRESHOLD_TOP) {
            cameraPosition.y += (transform.position.y - playerHeight) - (max.y - THRESHOLD_TOP);
        } else {
            cameraPosition.y += transform.position.y - (min.y + THRESHOLD_BOTTOM);

            // TODO: 場所によってカメラの移動限界を変えたい・・・。やっぱりカメラコリジョン必要？
            if (cameraPosition.y < 0) {
                cameraPosition.y = 0;
            }
        }

        // Y座標は MoveCameraY 関数によって緩やかに移動させる
        _targetCameraPositionY = cameraPosition.y;
    }
}