using UnityEngine;

// ↑ボタンを押すとプレイヤーの付近にあるものにインタラクトする
public class PlayerInteractAction : MonoBehaviour {
    const float INTERACTABLE_RADIUS = 1f;
    const int INTERACTION_INTERVAL_MSEC = 1000;

    int _intervalMsec;

    void Start() {
        _intervalMsec = 0;
    }

    void Update() {
        float axis = Input.GetAxisRaw("Vertical");
        int direction = (axis == 0) ? 0 : ((axis > 0) ? 1 : -1);
        if (direction == 1 && CanInteract) {
            Interact();
            _intervalMsec = INTERACTION_INTERVAL_MSEC;
        } else {
            _intervalMsec -= (int)(Time.deltaTime * 1000);
        }
    }

    bool CanInteract {
        get { return _intervalMsec <= 0; }
    }

    void Interact() {
        InteractiveObject interactiveObject = null;

        var hits = Physics2D.CircleCastAll(transform.position, INTERACTABLE_RADIUS, Vector2.zero, 1f);
        foreach (var hit in hits) {
            if (hit.collider == null) {
                continue;
            }

            interactiveObject = hit.collider.gameObject.GetComponent<InteractiveObject>();
            if (interactiveObject != null) {
                break;
            }
        }

        if (interactiveObject == null) {
            Debug.Log("Interact: InteractiveObject is not found...");
            return;
        }

        interactiveObject.Interact();
    }
}
