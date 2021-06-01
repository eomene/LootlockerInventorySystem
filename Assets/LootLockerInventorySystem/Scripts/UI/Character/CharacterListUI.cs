using LootLocker.InventorySystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LootLocker.InventorySystem
{
    [RequireComponent(typeof(ItemListPopulator))]
    public class CharacterListUI : UIScreen
    {
        [SerializeField] CharacterData characterData;
        [SerializeField] Button openCreateCharacterScreen;
        ItemListPopulator listPopulator;

        public override void Awake()
        {
            base.Awake();
            listPopulator = GetComponent<ItemListPopulator>();
            openCreateCharacterScreen.onClick.AddListener(OpenCreateCharacter);
        }


        public void OpenCreateCharacter()
        {
            menuManager?.OpenCreateCharacterUI();
        }

        public override void Open(Action onOpen = null)
        {
            LoadingManager.ShowLoadingScreen();
            characterData.GetCharacters(characters =>
            {
                listPopulator.PopulateList(characters);
                base.Open(onOpen);
                LoadingManager.HideLoadingScreen();
            });
        }
    }
}