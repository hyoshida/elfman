using Assets.Scripts.Utils;
using UnityEngine;

public class Stone : MonoBehaviour {
    void OnCollisionEnter2D(Collision2D collision) {
        CollisionUtil util = new CollisionUtil(collision);
        if (util.IsLayer("Ground")) {
            Destroy(gameObject);
        }
    }
}