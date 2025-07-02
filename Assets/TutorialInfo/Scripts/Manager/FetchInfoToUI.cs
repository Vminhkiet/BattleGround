using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FetchInfoToUI : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text moneyText;

    private void Start()
    {
        UserSession.Instance.Subscribe(UpdateUI);
    }

    private void UpdateUI()
    {
        string username = UserSession.Instance.userData.username;
        int money = UserSession.Instance.userData.money;

        usernameText.text = username;
        moneyText.text = money.ToString();
    }

}
