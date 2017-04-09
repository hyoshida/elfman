using System;
using System.Collections;
using System.IO;
using UnityEngine;

[Serializable]
public class FaceMaster {
    public uint code = 0;
    public string imagePath = "";

    Texture2D _imageTexture;

    public Sprite CreateImageSprite() {
        Texture2D texture = _imageTexture;
        return Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    // TODO: マスター内のテクスチャ作成処理は大体似てるので共通化したい
    public IEnumerator Load() {
        yield return LoadImageTexture();
    }

    public IEnumerator LoadImageTexture() {
        if (_imageTexture == null) {
            string path = Path.Combine(Application.streamingAssetsPath, imagePath);
            if (path.Contains("://")) {
                WWW www = new WWW(path);
                yield return www;
                _imageTexture = www.texture;
            } else {
                byte[] data = File.ReadAllBytes(path);
                _imageTexture = new Texture2D(1, 1);
                _imageTexture.LoadImage(data);
            }
        }
    }
}
