﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterItemUI : MonoBehaviour
{
    private int maxHealth = 5000;
    private int maxSpeed = 100;
    private int maxPower = 1000;
    [SerializeField] Color itemNotSelectedColor;
    [SerializeField] Color itemSelectedColor;

    [SerializeField] Image characterImage;
    [SerializeField] TMP_Text characterNameText;
    [SerializeField] Image characterHealthFill;
    [SerializeField] Image characterSpeedFill;
    [SerializeField] Image characterPowerFill;
    [SerializeField] TMP_Text characterPriceText;
    [SerializeField] Button characterPurchaseButton;

    [Space(20f)]
    [SerializeField] Button itemButton;
    [SerializeField] Image itemImage;
    [SerializeField] Outline itemOutline;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }

    public void SetCharacterImage(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }

    public void SetCharacterName(string name)
    {
        characterNameText.text = name;
    }
    public void SetCharacterHealth(float health)
    {
        characterHealthFill.fillAmount = health / maxHealth;
    }

    public void SetCharacterSpeed(float speed)
    {
        characterSpeedFill.fillAmount = speed / maxSpeed;
    }

    public void SetCharacterPower(float power)
    {
        characterPowerFill.fillAmount = power / maxPower;
    }

    public void SetCharacterPrice(int price)
    {
        characterPriceText.text = price.ToString();
    }
    public void SetCharacterAsPurchase()
    {
        characterPurchaseButton.gameObject.SetActive(false);

        itemButton.interactable = true;
        itemImage.color = itemNotSelectedColor;
    }

    public void OnItemPurchase(int itemIndex, int price, string characterName, UnityAction<int> purchaseCallback)
    {
        characterPurchaseButton.onClick.RemoveAllListeners();
        characterPurchaseButton.onClick.AddListener(async () => {
            // Gọi hàm mua từ FirestoreManager
            bool success = await UserSession.Instance.PurchaseCharacterAsync(characterName, price);

            if (success)
            {
                SetCharacterAsPurchase();
                purchaseCallback.Invoke(itemIndex);

            }
            else
            {
                Debug.Log("Purchase failed!");
            }
        });
    }

    public void OnItemSelect(int itemIndex, string characterName, UnityAction<int> selectCallback)
    {
        itemButton.interactable = true;
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(async () =>
        {
            await UserSession.Instance.SelectCharacterAsync(characterName);

            SpawnController.instance.getInstance().SpawnPlayer(characterName);
            selectCallback.Invoke(itemIndex);
        });

        itemImage.color = itemNotSelectedColor;
    }

    public void SelectItem()
    {
        itemOutline.enabled = true;
        itemImage.color = itemSelectedColor;
        itemButton.interactable = false;
    }

    public void DeselectItem()
    {
        itemOutline.enabled = false;
        itemImage.color = itemNotSelectedColor;
        itemButton.interactable = true;
    }
}
