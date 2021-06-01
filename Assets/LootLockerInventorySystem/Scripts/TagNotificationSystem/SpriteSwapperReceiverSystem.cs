using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.TagNotificationSystem
{
    public class SpriteSwapperReceiverSystem : MonoBehaviour, ICConsumeNotificationState
    {
        [SerializeField] NotificationReceiverMode notificationReceiverMode;
        [SerializeField] Image icon;
        [SerializeField] Sprite previousSprite;
        [SerializeField] Sprite newSprite;

        private void Awake()
        {
            previousSprite = icon.sprite;
        }

        public void NotificationEnded()
        {
            if (!icon) return;
            icon.sprite = previousSprite;
        }

        public void NotificationStarted(NotificationReceiverMode notificationReceiverMode)
        {
            if (this.notificationReceiverMode != notificationReceiverMode) return;
            if (!icon) return;
            icon.sprite = newSprite;
        }
    }
}