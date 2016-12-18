//
// Based on https://gist.github.com/tsubaki/e0406377a1b014754894
//
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T> {
    static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null) {
                    Debug.LogWarning(typeof(T) + "is nothing");
                }
            }

            return _instance;
        }
    }

    void Awake() {
        if (_instance == null) {
            _instance = (T)this;
        } else if (Instance != this) {
            Destroy(this);
        }
    }
}