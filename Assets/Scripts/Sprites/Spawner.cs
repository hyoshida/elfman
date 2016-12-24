using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    private const float CAMERA_OFFSET = 0f;

    [SerializeField]
    GameObject _enemyPrefab;

    Camera _camera;
    bool _spawnded;
    Sprite _sprite;
    GameObject _enemy;

    bool CanSpawn {
        get {
            return !_spawnded && IsVisible && (_enemy == null);
        }
    }

    bool IsVisible {
        get {
            float min = 0 - CAMERA_OFFSET;
            float max = 1 + CAMERA_OFFSET;
            Vector2 position = _camera.WorldToViewportPoint(transform.position);
            return ((min <= position.x && position.x <= max) && (min <= position.y && position.y <= max));
        }
    }

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        _sprite = spriteRenderer.sprite;

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update() {
        if (!IsVisible) {
            _spawnded = false;
        }

        if (CanSpawn) {
            Spawn();
            _spawnded = true;
        }
    }

    void OnWillRenderObject() {
        if (Camera.current.tag != _camera.tag) {
            return;
        }
    }

    void Spawn() {
        Vector3 position = transform.position;
        position.y -= _sprite.bounds.size.y / 2;
        _enemy = Instantiate(_enemyPrefab, position, transform.rotation);
        _enemy.transform.SetParent(transform.parent);
    }
}
