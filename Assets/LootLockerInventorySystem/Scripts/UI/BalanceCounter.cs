using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BalanceCounter : MonoBehaviour
{
    [SerializeField] UserData userData;

    Text textField;


    void Awake()
    {
        textField = GetComponent<Text>();
    }

    void Start()
    {
        userData.OnBalanceChanged += SetCounter;
    }

    void OnDestroy()
    {
        if (userData)
        {
            userData.OnBalanceChanged -= SetCounter;
        }
    }


    void Refresh()
    {
        userData.GetBalance(SetCounter);
    }

    void SetCounter(int balance)
    {
        textField.text = balance.ToString();
    }
}
