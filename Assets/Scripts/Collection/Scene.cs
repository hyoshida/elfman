using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Collection {
    public class Scene : MonoBehaviour {
        [SerializeField]
        GameObject _collectionItem;

        [SerializeField]
        GameObject _content;

        // Use this for initialization
        void Start() {
            StartCoroutine(LoadMasterAndCreateCollectionItems());
        }

        // Update is called once per frame
        void Update() {
        }

        IEnumerator LoadMasterAndCreateCollectionItems() {
            yield return Master.Load();
            CreateCollectionItems();
        }

        void CreateCollectionItems() {
            int index = 0;
            foreach (var stillMaster in Master.Instance.stillMasters) {
                CreateCollectionItem(stillMaster, index);
                index++;
            }
        }

        void CreateCollectionItem(StillMaster stillMaster, int index) {
            string path = Path.Combine(Application.streamingAssetsPath, stillMaster.imagePath);

            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);

            Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            GameObject item = Instantiate(_collectionItem);
            int col = index % 2;
            int row = index / 2;
            int offset = 75;
            int gap = 50;
            RectTransform rectTransform = item.transform as RectTransform;
            item.transform.position = new Vector3(offset + (rectTransform.rect.width + gap) * col, rectTransform.rect.height * row * -1);
            item.transform.SetParent(_content.transform, false);

            GameObject still = item.transform.Find("Image").gameObject;
            Image image = still.GetComponent<Image>();
            image.sprite = sprite;

            GameObject title = item.transform.Find("Title").gameObject;
            Text text = title.GetComponent<Text>();
            text.text = stillMaster.title;
        }
    }
}
