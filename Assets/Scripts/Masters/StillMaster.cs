﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class StillMaster {
    public uint code = 0;
    public uint stageCode = 0;
    public bool boss = false;
    public string imagePath = "";
    public string title = "";
    public List<string> texts = new List<string> { };

    Texture2D _imageTexture;

    public Sprite createImageSprite() {
        Texture2D texture = _imageTexture;
        return Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

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