using Cradaptive.MultipleTextureDownloadSystem;
using LootLocker.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.SlideShow
{

    public class SlideShowController : MonoBehaviour
    {
        [SerializeField] Image mainImage;
        [SerializeField] Button forwardBtn;
        [SerializeField] Button backBtn;
        [SerializeField] Text imageName;
        IListData[] listData;
        int currentIndex = 0;

        private void Awake()
        {
            forwardBtn.onClick.AddListener(Next);
            backBtn.onClick.AddListener(Prev);
        }

        public void SetUpList(IListData[] listData)
        {
            this.listData = listData;
            currentIndex = 0;
            if (listData.Length > currentIndex)
            {
                ICradaptiveTextureOwner textureOwner = listData[currentIndex] as ICradaptiveTextureOwner;
                if (textureOwner != null)
                {
                    textureOwner.OnTextureAvailable = SetUpImage;
                    CradaptiveTexturesSaver.QueueForDownload(textureOwner);
                }
                imageName.text = listData[currentIndex].name;
            }
        }

        public int GetIndex()
        {
            return currentIndex;
        }

        public void Next()
        {
            if (currentIndex < listData.Length - 1)
                currentIndex++;

            ICradaptiveTextureOwner textureOwner = listData[currentIndex] as ICradaptiveTextureOwner;
            if (textureOwner != null)
            {
                textureOwner.OnTextureAvailable = SetUpImage;
                CradaptiveTexturesSaver.QueueForDownload(textureOwner);
            }

            imageName.text = listData[currentIndex].name;

            forwardBtn.interactable = (currentIndex < listData.Length - 1);
            backBtn.interactable = true;
        }

        public void Prev()
        {
            if (currentIndex > 0)
                currentIndex--;

            ICradaptiveTextureOwner textureOwner = listData[currentIndex] as ICradaptiveTextureOwner;
            if (textureOwner != null)
            {
                textureOwner.OnTextureAvailable = SetUpImage;
                CradaptiveTexturesSaver.QueueForDownload(textureOwner);
            }

            imageName.text = listData[currentIndex].name;

            forwardBtn.interactable = true;
            backBtn.interactable = (currentIndex > 0);
        }

        void SetUpImage(Sprite sprite)
        {
            mainImage.sprite = sprite;
        }
    }
}