using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class LoadingManager : UIScreen
    {
        static LoadingManager Instance;
        bool isOpen;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }
        public static void ShowLoadingScreen(Action onShowLoadingScreen = null)
        {
            if (!Instance.isOpen)
            {
                Instance.isOpen = true;
                Instance.Open();
            }
        }


        public static void HideLoadingScreen()
        {
            if (Instance.isOpen)
            {
                Instance.Close();
                Instance.isOpen = false;
            }
        }
    }
}