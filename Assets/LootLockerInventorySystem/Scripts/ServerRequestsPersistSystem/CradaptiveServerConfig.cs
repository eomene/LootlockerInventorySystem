using UnityEngine;

namespace Cradaptive.ServerRequests
{
    [CreateAssetMenu(fileName = "CradaptiveServerConfig", menuName = "CradaptiveTools/CradaptiveServerConfig")]
    public class CradaptiveServerConfig : ScriptableObject
    {
        public enum FailureHandling { RetryAfterSomeSeconds, TellPlayer }
        public FailureHandling failureHandlingMethod;
        public float secondsBeforeRetry = 1;
        public float maxRetryCount = 10;
        public string persitenceSaveKey = "Cradaptive.ServerRequests.BackgroundServerRequestDispatcher.pendingRequests";
        public bool persistentRequestAfterClosingApp;
        public bool broadcastResponseToListeners;
    }
}