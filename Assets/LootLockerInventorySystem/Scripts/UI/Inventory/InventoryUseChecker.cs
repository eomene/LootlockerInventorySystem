using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LootLocker.InventorySystem
{
    public class InventoryUseChecker : MonoBehaviour, IListItemCallBack
    {
        [SerializeField] CharacterData characterData;

        public GameObject highlightObject;

        public void OnButtonClicked()
        {
        }


        public void ListItemInitialised(IListData listData)
        {
            CradaptiveLootLockerInventoryItem data = listData as CradaptiveLootLockerInventoryItem;
            if (data != null)
            {
                if (characterData.CurrentCharacter.equippableContexts.FirstOrDefault(x => x.name == data.asset.context) != null)
                    highlightObject?.SetActive(false);
                else
                    highlightObject?.SetActive(true);
            }

        }

    }
}