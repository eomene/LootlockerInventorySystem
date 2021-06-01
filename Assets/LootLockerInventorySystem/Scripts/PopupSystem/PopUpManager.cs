using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.PopUps
{
    public static class PopUpManager
    {
        const int CANVAS_SORT_ORDER = 10000;
        const float MARGIN = 10;

        static RectTransform popUpParent;

        static PopUp PopUpPrefab => Resources.Load<PopUp>("PopUp");// TODO: Get the reference from somewhere that isn't Resources?


        public static void ShowPopUp(PopUpData data)
        {
            if (!popUpParent)
            {
                CreateCanvas();
            }

            PopUp popUp = CreatePopup(data);

            RectTransform popUpTransform = popUp.GetComponent<RectTransform>();
            popUpTransform.SetParent(popUpParent);

            popUpTransform.anchorMin = Vector2.zero;
            popUpTransform.anchorMax = Vector2.zero;
        }


        static void CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Popup Canvas");
            GameObject.DontDestroyOnLoad(canvasGO);

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CANVAS_SORT_ORDER;

            GraphicRaycaster graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();

            GameObject layoutGroupGO = new GameObject("Layout Group");
            popUpParent = layoutGroupGO.AddComponent<RectTransform>();
            popUpParent.SetParent(canvas.transform);
            popUpParent.anchorMin = Vector2.zero;
            popUpParent.anchorMax = Vector2.one;

            popUpParent.offsetMin = new Vector2(MARGIN, MARGIN);
            popUpParent.offsetMax = new Vector2(-MARGIN, -MARGIN);

            VerticalLayoutGroup layoutGroup = popUpParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.LowerLeft;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;

            layoutGroup.spacing = MARGIN;
        }

        static PopUp CreatePopup(PopUpData data)
        {
            PopUp popUp = GameObject.Instantiate(PopUpPrefab, Vector3.zero, Quaternion.identity, parent: null);
            popUp.SetUp(data);

            return popUp;
        }
    }
}