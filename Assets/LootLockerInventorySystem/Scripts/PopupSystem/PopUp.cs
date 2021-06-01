using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.PopUps
{
    public class PopUp : MonoBehaviour
    {
        [SerializeField] Text textField;
        [SerializeField] Image image;


        public void SetUp(PopUpData data)
        {
            name = $"PopUp: {data.text}";

            textField.text = data.text;
            image.sprite = data.image;

            Color imageTint = image.color;
            imageTint.a = data.image ? 1 : 0;
            image.color = imageTint;
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}