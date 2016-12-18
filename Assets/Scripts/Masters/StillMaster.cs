using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class StillMaster {
    public uint stageCode = 0;
    public string imagePath = "";
    public string title = "";
    public List<string> texts = new List<string> { };

    Texture2D _imageTexture;

    public Texture2D imageTexture {
        get {
            return _imageTexture;
        }
    }

    public IEnumerator LoadImageTexture() {
        if (!imageTexture) {
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