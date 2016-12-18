using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ApplicationScene : MonoBehaviour {
    bool _loaded = false;

    public IEnumerator WaitForLoad() {
        yield return new WaitWhile(() => !_loaded);
    }

    void Awake() {
        // TODO: ロード中はフェードが明けないようにしたい
        StartCoroutine(Load());
    }

    IEnumerator Load() {
        if (!_loaded) {
            yield return Master.Load();
            PlayerVO.Instance.Load();

            _loaded = true;
        }
    }
}
