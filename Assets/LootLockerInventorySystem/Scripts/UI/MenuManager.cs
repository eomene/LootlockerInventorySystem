using LootLocker.InventorySystem;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public UIScreen[] screens;
    [SerializeField] InventoryData inventoryData;
    [SerializeField] CharacterData characterData;
   // [SerializeField] int defaultScreen = 1;

    void Start()
    {
      //  OpenScreen(defaultScreen);
    }

    public void OpenScreen(int index)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            if (i != index)
            {
                screens[i]?.Close();
            }
        }

        if (index >= 0 && index < screens.Length)
            screens[index]?.Open();
    }

    public void CloseScreen(int index)
    {
        if (index > 0 && index < screens.Length)
            screens[index]?.Close();
    }

    public void CloseAllScreens()
    {
        for (int i = 0; i < screens.Length; i++)
                screens[i]?.Close();
    }

    public void OpenInventoryUI()
    {
        OpenScreen(0);
    }

    public void OpenCreateCharacterUI()
    {
        OpenScreen(2);
    }
    public void OpenCharacterListUI()
    {
        OpenScreen(1);
    }
}
