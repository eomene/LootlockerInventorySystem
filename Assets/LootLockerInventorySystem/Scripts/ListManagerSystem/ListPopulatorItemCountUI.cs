using UnityEngine;
using UnityEngine.UI;

namespace LootLocker.InventorySystem
{
    [RequireComponent(typeof(Text))]
    public class ListPopulatorItemCountUI : MonoBehaviour
    {
        [SerializeField] ListPopulator listPopulator;
        [SerializeField] string prefix = "Item count: ";

        Text textField;


        void Awake()
        {
            textField = GetComponent<Text>();
        }

        void Start()
        {
            SetItemCount(listPopulator.Items.Count);
            listPopulator.OnItemCountChanged += SetItemCount;
        }

        void SetItemCount(int itemCount)
        {
            textField.text = prefix + itemCount.ToString();
        }
    }
}