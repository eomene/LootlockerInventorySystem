using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.TagNotificationSystem
{
    public class ColorModifierReceiverSystem : MonoBehaviour, ICConsumeNotificationState
    {
        [SerializeField] NotificationReceiverMode notificationReceiverMode;
        [SerializeField] Image icon;
        [SerializeField] Color colorSwap;
        Color previousColor;

        private void Awake()
        {
            previousColor = icon.color;
        }

        public void NotificationEnded()
        {
            if (!icon) return;
            icon.color = previousColor;
        }

        public void NotificationStarted(NotificationReceiverMode notificationReceiverMode)
        {
            if (this.notificationReceiverMode != notificationReceiverMode) return;
            if (!icon) return;
            icon.color = colorSwap;
        }
    }
}