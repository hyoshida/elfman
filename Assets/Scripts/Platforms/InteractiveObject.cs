using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractiveObject : MonoBehaviour {
    virtual public void Interact() {
        Debug.LogError("Please implement!");
    }
}
