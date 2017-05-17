using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractiveObject : MonoBehaviour {
    [SerializeField]
    GameObject _label;

    public bool CanInteract {
        set {
            _label.SetActive(value);
        }
    }

    virtual public void Interact() {
        Debug.LogError("Please implement!");
    }
}
