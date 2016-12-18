using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collection {
    public class Scene : ApplicationScene {
        [SerializeField]
        GameObject _collectionItem;

        [SerializeField]
        GameObject _content;

        // Use this for initialization
        IEnumerator Start() {
            yield return WaitForLoad();
            CreateCollectionItems();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetButtonDown("Cancel")) {
                GameManager.Instance.GotoTitle();
            }
        }

        void CreateCollectionItems() {
            List<uint> unlockedStillCodes = PlayerVO.Instance.unlockedStillCodes;
            foreach (var stillMaster in Master.Instance.stillMasters) {
                if (unlockedStillCodes.IndexOf(stillMaster.code) == -1) {
                    continue;
                }
                CreateCollectionItem(stillMaster);
            }

            // なんかviewportのサイズがおかしくなるので直す
            RectTransform viewportTransform = _content.transform.parent as RectTransform;
            viewportTransform.sizeDelta = new Vector2(-17, 0);
        }

        void CreateCollectionItem(StillMaster stillMaster) {
            GameObject item = Instantiate(_collectionItem, _content.transform);

            GameObject still = item.transform.Find("Image").gameObject;
            Image image = still.GetComponent<Image>();
            image.sprite = stillMaster.createImageSprite();

            GameObject title = item.transform.Find("Title").gameObject;
            Text text = title.GetComponent<Text>();
            text.text = stillMaster.title;
        }
    }
}