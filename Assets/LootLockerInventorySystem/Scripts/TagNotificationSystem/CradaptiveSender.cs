using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cradaptive.TagNotificationSystem
{
    public class OnItemNotifiedOfType : UnityEvent<EventNotificationTypes> { }

    public class CradaptiveSender : MonoBehaviour
    {
        public static OnItemNotifiedOfType onItemNotifiedOfType = new OnItemNotifiedOfType();
        public static OnItemNotifiedOfType onEndedItemNotificationOfType = new OnItemNotifiedOfType();

        /// <summary>
        /// This sends a universal notification to whoever is listening
        /// </summary>
        /// <param name="tag"></param>
        public static void SendNotificationToTags(EventNotificationTypes tag)
        {
            if (tag == null) return;
            onItemNotifiedOfType?.Invoke(tag);
        }

        public static void EndNotificationToTags(EventNotificationTypes tag)
        {
            if (tag == null) return;
            onEndedItemNotificationOfType?.Invoke(tag);
        }

    }
}