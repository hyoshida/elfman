﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public const int SPEED = -3;

    Rigidbody2D _rigidbody2D;

    // Use this for initialization
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        _rigidbody2D.velocity = new Vector2(SPEED, _rigidbody2D.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Bullet") {
            Destroy(collider.gameObject);
            Destroy(gameObject);
        }
    }
}
