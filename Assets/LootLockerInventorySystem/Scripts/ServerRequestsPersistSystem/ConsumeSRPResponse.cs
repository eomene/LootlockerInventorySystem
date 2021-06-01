using Cradaptive.ServerRequests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cradaptive.ServerRequests
{
    public interface IConsumeSRPResponse
    {
        void ConsumeResponse(object srpresponse);
    }

    public interface IConsumeSRPPendingRequests
    {
        void ConsumeRequests(object srpresponse);
    }

    public class ConsumeSRPResponse : MonoBehaviour
    {
        private void OnEnable()
        {
            BackgroundServerRequestDispatcher.onBackgroundServerDataAvailable.AddListener(ConsumeResponse);
            BackgroundServerRequestDispatcher.onPendingRequestData.AddListener(ConsumeRequests);
        }

        private void OnDisable()
        {
            BackgroundServerRequestDispatcher.onBackgroundServerDataAvailable.RemoveListener(ConsumeResponse);
            BackgroundServerRequestDispatcher.onPendingRequestData.RemoveListener(ConsumeRequests);
        }

        ///I know responses might come from the background requester,so this listens for it and handles it appropraitely. Maybe also cache this, 
        ///so it can be applied when things have been moved around.
        ///This is a better way for listening for when background dispatcher has finished its action it can then pass it to whoever implements this interface on the gameobject
        void ConsumeResponse(object srpresponse)
        {
            GetComponent<IConsumeSRPResponse>()?.ConsumeResponse(srpresponse);
        }
        void ConsumeRequests(object srpresponse)
        {
            GetComponent<IConsumeSRPPendingRequests>()?.ConsumeRequests(srpresponse);
        }
    }
}