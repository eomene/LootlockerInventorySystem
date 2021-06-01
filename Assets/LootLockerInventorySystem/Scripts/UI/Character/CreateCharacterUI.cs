using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using Cradaptive.SlideShow;

namespace LootLocker.InventorySystem
{
    public class CreateCharacterUI : UIScreen
    {
        public CharacterData characterData;
        public Button createCharacter;
        public Button back;
        public InputField characterName;
        public SlideShowController slideShowController;
        public Toggle isDefault;
        CradaptiveLootLockerCharacter_Types[] lootLockerCharacter_Types;


        public override void Awake()
        {
            base.Awake();
            back.onClick.AddListener(GoBack);
            createCharacter.onClick.AddListener(CreateCharacter);
        }

        public void OpenCreateCharacter()
        {
            menuManager?.OpenCreateCharacterUI();
        }

        public override void Open(Action onOpen = null)
        {
            LoadingManager.ShowLoadingScreen();
            characterName.text = "";
            characterData.GetCharacterClasses((response) =>
            {
                string json = JsonConvert.SerializeObject(response);
                lootLockerCharacter_Types = JsonConvert.DeserializeObject<CradaptiveLootLockerCharacter_Types[]>(json);
                slideShowController?.SetUpList(lootLockerCharacter_Types);
                LoadingManager.HideLoadingScreen();
                base.Open(onOpen);
            });

        }

        public void CreateCharacter()
        {
            LoadingManager.ShowLoadingScreen();
            LootLockerCharacter_Types selectedClass = lootLockerCharacter_Types[slideShowController.GetIndex()];

            characterData.CreateCharacter(selectedClass.id.ToString(), characterName.text, isDefault, onCompleted: () =>
            {
                menuManager?.OpenCharacterListUI();
                LoadingManager.HideLoadingScreen();
            });
        }

        public void GoBack()
        {
            menuManager?.OpenCharacterListUI();
        }

    }
}