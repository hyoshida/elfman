using UnityEngine;

// ↑ボタンを押すとプレイヤーの付近にあるものにインタラクトする
public class PlayerInteractAction : MonoBehaviour {
    const int INTERACTION_INTERVAL_MSEC = 1000;

    int _intervalMsec;
    InteractiveObject _interactiveObject;

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

    void OnTriggerEnter2D(Collider2D collider2d) {
        var interactiveObject = collider2d.gameObject.GetComponent<InteractiveObject>();
        if (interactiveObject == null) {
            return;
        }

        _interactiveObject = interactiveObject;

        Debug.Log("Found a interactive object!");
    }

    void OnTriggerExit2D(Collider2D collider2d) {
        _interactiveObject = null;
    }


    bool CanInteract {
        get { return _intervalMsec <= 0; }
    }

    void Interact() {
        if (_interactiveObject == null) {
            Debug.Log("Interact: InteractiveObject is not found...");
            return;
        }

        _interactiveObject.Interact();
    }
}
