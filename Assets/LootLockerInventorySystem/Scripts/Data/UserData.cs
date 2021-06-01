using System;
using Cradaptive.ServerRequests;
using LootLocker.InventorySystem;
using LootLocker.Requests;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "InventorySystem/UserData")]
public class UserData : ScriptableObject
{
    [SerializeField] InventoryData inventoryData;
    public Action<int> OnBalanceChanged;

    void OnEnable()
    {
        if (inventoryData)
        {
            inventoryData.OnInventoryChanged += InvokeOnBalanceChanged;
        }
    }

    void OnDisable()
    {
        if (inventoryData)
        {
            inventoryData.OnInventoryChanged -= InvokeOnBalanceChanged;
        }
    }

    void OnValidate()
    {
        if (inventoryData)
        {
            inventoryData.OnInventoryChanged -= InvokeOnBalanceChanged;
            inventoryData.OnInventoryChanged += InvokeOnBalanceChanged;
        }
    }

    /// <summary>
    /// Retrieves your balance from lootlocker
    /// </summary>
    /// <param name="onCompleted"></param>
    public void GetBalance(Action<int> onCompleted)
    {
        //var request = new CriticalServerGetRequest<LootLockerBalanceResponse, int>()
        //{
        //    makeRequest = onResponseReceived => LootLockerSDKManager.GetBalance(onResponseReceived),
        //    responseIsSuccesful = response => response.success,
        //    extractResult = response => response.balance ?? 0
        //};

        //CriticalServerRequestDispatcher.Execute(request, onCompleted);
    }

    void InvokeOnBalanceChanged()
    {
        GetBalance(balance => OnBalanceChanged?.Invoke(balance));
    }
}
