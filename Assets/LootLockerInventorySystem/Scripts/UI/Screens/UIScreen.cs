using System;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    CanvasGroup canvasGroup;
    protected MenuManager menuManager;

    public virtual void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public virtual void Open(Action onOpen = null)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
      //  gameObject.SetActive(true);
    }

    public virtual void Close(Action onClose = null)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
      //  gameObject.SetActive(false);
    }
}
