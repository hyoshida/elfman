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
            if (Input.GetButtonDown("Cancel")) {
                GameManager.Instance.GotoTitle();
            }
        }

        IEnumerator LoadMasterAndCreateCollectionItems() {
            yield return Master.Load();
            CreateCollectionItems();

            // なんかviewportのサイズがおかしくなるので直す
            RectTransform viewportTransform = _content.transform.parent as RectTransform;
            viewportTransform.sizeDelta = new Vector2(-17, 0);
        }

        void CreateCollectionItems() {
            foreach (var stillMaster in Master.Instance.stillMasters) {
                CreateCollectionItem(stillMaster);
            }
        }

        void CreateCollectionItem(StillMaster stillMaster) {
            string path = Path.Combine(Application.streamingAssetsPath, stillMaster.imagePath);

            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);

            Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            GameObject item = Instantiate(_collectionItem, _content.transform);

            GameObject still = item.transform.Find("Image").gameObject;
            Image image = still.GetComponent<Image>();
            image.sprite = sprite;

            GameObject title = item.transform.Find("Title").gameObject;
            Text text = title.GetComponent<Text>();
            text.text = stillMaster.title;
        }
    }
}
