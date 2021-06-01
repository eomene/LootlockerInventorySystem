using System;
using Cradaptive.ServerRequests;
using LootLocker.Requests;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] InventoryData inventoryData;
        [SerializeField] string testID;
        [SerializeField] MenuManager menuManager;

        void Start()
        {
            LoadingManager.ShowLoadingScreen();

            string id = string.IsNullOrEmpty(testID) ? SystemInfo.deviceUniqueIdentifier : testID;

            var startSessionRequest = new CriticalServerRequest<LootLockerSessionResponse>()
            {
                makeRequest = onResponseReceived => LootLockerSDKManager.StartSession(id, onResponseReceived),
                responseIsSuccesful = response => response.success
            };

            CriticalServerRequestDispatcher.Execute(startSessionRequest, onCompleted: () =>
            {
                inventoryData.FetchInventoryFromServer(onFinishedFetch: () =>
                {
                    LoadingManager.HideLoadingScreen();
                    menuManager?.OpenCharacterListUI();
                });
            });
        }
    }
}