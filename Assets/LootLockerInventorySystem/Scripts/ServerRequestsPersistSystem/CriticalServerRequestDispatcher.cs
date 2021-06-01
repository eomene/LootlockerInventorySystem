using System;
using System.Collections;
using LootLocker;
using UnityEngine;

namespace Cradaptive.ServerRequests
{
    public static class CriticalServerRequestDispatcher
    {
        public static CradaptiveServerConfig config;
        delegate void RequestEndedHandler<TResponse>(TResponse response);

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            config = Resources.Load<CradaptiveServerConfig>("CradaptiveSRPSConfig/CradaptiveSRPSConfig");
        }

        public static void Execute<TResponse>(IServerRequest<TResponse> request, Action onCompleted)
        {
            Execute(request, _ => onCompleted());
        }

        public static void Execute<TResponse, TResult>(IServerGetRequest<TResponse, TResult> request, Action<TResult> onCompleted)
        {
            Execute(request, response =>
            {
                TResult result = request.ExtractResult(response);
                onCompleted(result);
            });
        }


        static void Execute<TResponse>(IServerRequest<TResponse> request, RequestEndedHandler<TResponse> onRequestSuccess)
        {
            StartCoroutine(DoRequestCoroutine(request, onRequestSuccess));
        }


        static void StartCoroutine(IEnumerator coroutine)
        {
            LootLockerServerManager.I.StartCoroutine(coroutine);    // TODO: Do this with another object to remove LootLocker dependency. Alternatevely, async code could be used instead of a coroutine.
        }

        static IEnumerator DoRequestCoroutine<TResponse>(IServerRequest<TResponse> request, RequestEndedHandler<TResponse> onRequestSuccess)
        {
            TResponse serverResponse = default;
            bool responseIsSuccesful = false;

            int attemptCount = 0;

            do
            {
                bool receivedResponse = false;

                attemptCount++;
                request.MakeRequest(response =>
                {
                    serverResponse = response;
                    receivedResponse = true;
                });

                yield return new WaitUntil(() => receivedResponse);

                responseIsSuccesful = request.ResponseIsSuccesful(serverResponse);

                if (!responseIsSuccesful)
                {
                    yield return new WaitForSecondsRealtime(config.secondsBeforeRetry);
                }
            }
            while (!responseIsSuccesful && attemptCount < config.maxRetryCount);

            if (responseIsSuccesful)
            {
                onRequestSuccess(serverResponse);
            }
            else
            {
                // TODO: Give proper feedback
                Debug.LogError("Gave up");
            }
        }
    }


    public struct CriticalServerRequest<TResponse> : IServerRequest<TResponse>
    {
        public Action<Action<TResponse>> makeRequest;
        public Func<TResponse, bool> responseIsSuccesful;

        public void MakeRequest(Action<TResponse> onResponseReceived)
        {
            makeRequest(onResponseReceived);
        }

        public bool ResponseIsSuccesful(TResponse response)
        {
            return responseIsSuccesful(response);
        }
    }

    public struct CriticalServerGetRequest<TResponse, TResult> : IServerGetRequest<TResponse, TResult>
    {
        public Action<Action<TResponse>> makeRequest;
        public Func<TResponse, bool> responseIsSuccesful;
        public Func<TResponse, TResult> extractResult;


        public void MakeRequest(Action<TResponse> onResponseReceived)
        {
            makeRequest(onResponseReceived);
        }

        public bool ResponseIsSuccesful(TResponse response)
        {
            return responseIsSuccesful(response);
        }

        public TResult ExtractResult(TResponse response)
        {
            return extractResult(response);
        }
    }
}