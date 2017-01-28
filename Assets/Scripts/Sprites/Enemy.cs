using UnityEngine;

public class Enemy : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Bullet") {
            Destroy(collider.gameObject);
            Destroy(gameObject);
        }

        if (collider.tag == "Sword") {
            Destroy(gameObject);
        }
    }
}
