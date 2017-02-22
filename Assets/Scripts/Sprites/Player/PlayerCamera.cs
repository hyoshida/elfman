using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerCamera : MonoBehaviour {
    Camera _camera;
    Renderer _renderer;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.battleMode == BattleMode.Stage) {
            MoveCamera();
        }
    }

    void MoveCamera() {
        Vector3 cameraPosition = _camera.transform.position;

        // Playerの位置から右に少し移動した位置を画面中央にする
        cameraPosition.x = transform.position.x;

        const float THRESHOLD_TOP = 5f;
        const float THRESHOLD_BOTTOM = 2.15f;
        var playerHeight = _renderer.bounds.size.y;
        Vector2 min = _camera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = _camera.ViewportToWorldPoint(new Vector2(1, 1));
        if ((transform.position.y - playerHeight) > max.y - THRESHOLD_TOP) {
            cameraPosition.y += (transform.position.y - playerHeight) - (max.y - THRESHOLD_TOP);
        } else if (transform.position.y < min.y + THRESHOLD_BOTTOM) {
            cameraPosition.y += transform.position.y - (min.y + THRESHOLD_BOTTOM);

            // TODO: 場所によってカメラの移動限界を変えたい・・・。やっぱりカメラコリジョン必要？
            if (cameraPosition.y < 0) {
                cameraPosition.y = 0;
            }
        }

        _camera.transform.position = cameraPosition;
    }
}