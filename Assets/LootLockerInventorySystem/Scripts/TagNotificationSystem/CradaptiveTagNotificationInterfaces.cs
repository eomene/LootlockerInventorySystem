using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cradaptive.TagNotificationSystem
{
    public interface ICConsumeNotificationState
    {
        void NotificationStarted(NotificationReceiverMode notificationReceiverMode);
        void NotificationEnded();
    }

    [System.Serializable]
    public class EventNotificationTypes
    {
        public string category;
        public string tag;
    }

    public interface ITaggedData
    {
        EventNotificationTypes tag { get; }
    }
}