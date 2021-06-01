using System;
using System.Collections;
using System.Reflection;
using LootLocker;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Cradaptive.ServerRequests
{
    public class OnBackgroundServerDataAvailable : UnityEvent<BackgroundRequestsResponse> { }
    public class OnPendingRequestDataAvailable : UnityEvent<object> { }
    public class BackgroundRequestsResponse
    {
        public bool success;
        public object jsonResponse;
        public object requestResponse;
    }

    public static class BackgroundServerRequestDispatcher
    {
        public static OnBackgroundServerDataAvailable onBackgroundServerDataAvailable = new OnBackgroundServerDataAvailable();
        public static OnPendingRequestDataAvailable onPendingRequestData = new OnPendingRequestDataAvailable();
        static CradaptiveServerConfig config;
        static readonly Queue pendingRequests = new Queue();
        static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };
        static Coroutine queueExecutionCoroutine = null;
        static bool IsExecutingQueue => queueExecutionCoroutine != null;


        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            // We must make sure to reset the static state to support entering play mode without a domain reload.
            pendingRequests.Clear();
            queueExecutionCoroutine = null;
            config = Resources.Load<CradaptiveServerConfig>("CradaptiveSRPSConfig/CradaptiveSRPSConfig");
            LoadSavedPendingRequests();
        }

        /// <summary>
        /// This allows you to Queue server calls you do not think would be important to get an instant response from the server
        /// You can just queue it and forget about it. The system handles it and makes sure it keeps trying it in the background 
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        public static void Queue<TRequest>(IBackgroundServerRequest<TRequest> request)
        {
            pendingRequests.Enqueue(request);
            SavePendingRequests();
            StartQueueExecution();
        }

        public static void StartQueueExecution()
        {
            if (!IsExecutingQueue)
            {
                queueExecutionCoroutine = StartCoroutine(ExecuteQueueCoroutine());
            }
        }


        static IEnumerator ExecuteQueueCoroutine()
        {
            try
            {
                int attemptCount = 0;

                while (pendingRequests.Count > 0 && attemptCount < config.maxRetryCount)
                {
                    attemptCount++;

                    object nextRequest = pendingRequests.Peek();

                    bool receivedResponse = false;
                    bool wasSuccesful = false;

                    Execute(nextRequest, requestWasSuccesful =>
                    {
                        receivedResponse = true;
                        wasSuccesful = requestWasSuccesful;

                        if (requestWasSuccesful)
                        {
                            pendingRequests.Dequeue();
                            SavePendingRequests();
                        }
                    });

                    yield return new WaitUntil(() => receivedResponse);

                    if (!wasSuccesful)
                    {
                        yield return new WaitForSecondsRealtime(config.secondsBeforeRetry);
                    }
                }
            }
            finally
            {
                // Make sure that we clear this, even if an exception is raised
                queueExecutionCoroutine = null;
            }
        }

        static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return LootLockerServerManager.I.StartCoroutine(coroutine);
        }

        static void Execute(object request, Action<bool> onRequestEnded)
        {
            // Because different requests have different response types, we need to do some tricks to pass stuff without caring what type the response is.
            // That's why we use reflection here. It's not pretty, but I couldn't figure out a better way.

            Type requestType = request.GetType();

            MethodInfo makeRequest_Method = requestType.GetMethod(nameof(IServerRequest<int>.MakeRequest)); // It doesn't matter what type we use as generic here, but we must pass a type. Hence, int.
            MethodInfo requestWasSuccesful_Method = requestType.GetMethod(nameof(IServerRequest<int>.ResponseIsSuccesful));

            Action<object> onReceivedResponse = (response) =>
            {
                bool requestWasSuccesful = (bool)requestWasSuccesful_Method.Invoke(
                    request, new object[] { response });
                //We can now broadcast this to everyone, and they can handle it no matter how they want
                if (config.broadcastResponseToListeners)
                {
                    BackgroundRequestsResponse backgroundRequestsResponse = new BackgroundRequestsResponse { requestResponse = request, jsonResponse = response, success = requestWasSuccesful };
                    onBackgroundServerDataAvailable?.Invoke(backgroundRequestsResponse);
                }
                onRequestEnded(requestWasSuccesful);
            };

            makeRequest_Method.Invoke(request, new object[] { onReceivedResponse });
        }


        static string SerializeRequest(object request)
        {
            return JsonConvert.SerializeObject(request, Formatting.Indented, serializerSettings);
        }

        static object DeserializeRequest(string json)
        {
            return JsonConvert.DeserializeObject(json, serializerSettings);
        }

        static void SavePendingRequests()
        {
            if (config.persistentRequestAfterClosingApp)
            {
                string json = JsonConvert.SerializeObject(pendingRequests.ToArray(), serializerSettings);
                PlayerPrefs.SetString(config.persitenceSaveKey, json);
            }
        }

        static void LoadSavedPendingRequests()
        {
            if (config.persistentRequestAfterClosingApp && PlayerPrefs.HasKey(config.persitenceSaveKey))
            {
                object[] savedRequests = JsonConvert.DeserializeObject(PlayerPrefs.GetString(config.persitenceSaveKey), serializerSettings) as object[];

                foreach (object request in savedRequests)
                {
                    onPendingRequestData?.Invoke(request);
                    pendingRequests.Enqueue(request);
                }
                Debug.LogError("Found " + pendingRequests.Count + "requests pending, retrying");
            }
        }
    }

    public interface IBackgroundServerRequest<TResponse> : IServerRequest<TResponse>
    {
    }
}