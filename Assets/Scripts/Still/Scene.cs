using UnityEngine;

namespace Still {
    public class Scene : MonoBehaviour {
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetButtonDown("Fire1")) {
                GameManager.Instance.GotoTitle();
            }
        }
    }
}