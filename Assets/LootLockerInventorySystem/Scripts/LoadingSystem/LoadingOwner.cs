using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class LoadingOwner : MonoBehaviour
    {
        Action onOpen;
        Action onClose;

        // Start is called before the first frame update
        public void SetOnOpenAction(Action onOpen)
        {
            this.onOpen = onOpen;
        }

        public void SetOnCloseAction(Action onClose)
        {
            this.onClose = onClose;
        }

        public void OnOpen()
        {
            this.onOpen?.Invoke();
            this.onOpen = null;
        }

        public void OnClose()
        {
            this.onClose?.Invoke();
            this.onClose = null;
        }

    }
}