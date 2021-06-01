using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class CharacterListItem : MonoBehaviour, IListItemCallBack
    {
        [SerializeField] CharacterData characterData;

        LootLockerCharacterData character;
         

        public void OnButtonClicked()
        {
            LoadingManager.ShowLoadingScreen();
            characterData.SetCurrentCharacter(character, () =>
            {
                GetComponentInParent<MenuManager>().OpenInventoryUI();
                LoadingManager.HideLoadingScreen();
            });

        }

        public void ListItemInitialised(IListData listData)
        {
            character = listData as LootLockerCharacterData;
        }
    }
}
