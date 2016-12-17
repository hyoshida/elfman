using UnityEngine;
using System;
using System.Collections;

public class FadeManager : SingletonMonoBehaviour<FadeManager> {
    public const float DEFAULT_INTERVAL = 0.25f;

    Texture2D _texture;
    bool _isFading;
    float _alpha = 0f;

    void Awake() {
        if (this != Instance) {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _texture = new Texture2D(32, 32, TextureFormat.RGB24, false);
        _texture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0, false);
        _texture.SetPixel(0, 0, Color.white);
        _texture.Apply();
    }

    void OnGUI() {
        if (!_isFading) {
            return;
        }

        // 透明度を更新して黒テクスチャを描画
        GUI.color = new Color(0, 0, 0, _alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
    }

    public void Run(Action onFadeOut, float interval = DEFAULT_INTERVAL) {
        StartCoroutine(TransitionScene(onFadeOut, interval));
    }

    IEnumerator TransitionScene(Action onFadeOut, float interval) {
        _isFading = true;

        // フェードアウト
        float time = 0;
        while (time <= interval) {
            _alpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        // フェードアウトしきったら実行する
        onFadeOut();

        // フェードイン
        time = 0;
        while (time <= interval) {
            _alpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        _isFading = false;
    }
}