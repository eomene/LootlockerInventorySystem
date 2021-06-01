using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cradaptive.TagNotificationSystem
{
    public enum NotificationReceiverMode { SameTagAndCategory, SameCategoryDifferentTag };
    public class CradaptiveNotificationReceiver : MonoBehaviour
    {
        public EventNotificationTypes currentNotificationTag;
        ICConsumeNotificationState[] consumers;
        public bool receiveEvents;

        private void Awake()
        {
            consumers = GetComponents<ICConsumeNotificationState>();
        }

        public void OnNotificationReceived(EventNotificationTypes tag)
        {
            if (!receiveEvents) return;
            for (int i = 0; i < consumers.Length; i++)
                consumers[i]?.NotificationStarted(NotificationMode(tag));
        }

        public void OnNotificationEnded(EventNotificationTypes tag)
        {
            if (!receiveEvents) return;
            for (int i = 0; i < consumers.Length; i++)
                consumers[i]?.NotificationEnded();
        }

        public NotificationReceiverMode NotificationMode(EventNotificationTypes tag)
        {
            NotificationReceiverMode mode = NotificationReceiverMode.SameTagAndCategory;
            if ((tag != null && tag.tag.ToLower() != currentNotificationTag.tag.ToLower() && tag.category.ToLower() == currentNotificationTag.category.ToLower()))
                mode = NotificationReceiverMode.SameCategoryDifferentTag;
            else if (tag != null && tag.tag.ToLower() == currentNotificationTag.tag.ToLower() && tag.category.ToLower() == currentNotificationTag.category.ToLower())
                mode = NotificationReceiverMode.SameTagAndCategory;
            return mode;
        }

        void OnEnable()
        {
            CradaptiveSender.onItemNotifiedOfType.AddListener(OnNotificationReceived);
            CradaptiveSender.onEndedItemNotificationOfType.AddListener(OnNotificationEnded);
        }

        void OnDisable()
        {
            CradaptiveSender.onItemNotifiedOfType.AddListener(OnNotificationReceived);
            CradaptiveSender.onEndedItemNotificationOfType.AddListener(OnNotificationEnded);
        }
    }
}