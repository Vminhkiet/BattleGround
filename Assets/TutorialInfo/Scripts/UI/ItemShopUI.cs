﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;

class ItemShopUI : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] float itemSpacingCol;
    [SerializeField] float itemSpacingRow;
    float itemHeight;
    float itemWidth;

    [Header("UI elements")]
    [SerializeField] Transform ShopMenu;
    [SerializeField] Transform ShopItemsContainer;
    [SerializeField] GameObject itemPrefab;

    [Space(20)]
    [SerializeField] ItemDatabase itemDB;

    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button openShopButton;
    [SerializeField] Button closeShopButton;

    // Start is called before the first frame update
    void Start()
    {
        AddShopEvents();
        UserSession.Instance.Subscribe(GenerateShopItemUI);
    }

    void GenerateShopItemUI()
    {
        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        itemWidth = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        for (int i = 0; i < itemDB.ItemCount; i++)
        {
            Item item = itemDB.GetItem(i);
            ItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<ItemUI>();

            if ((i & 1) == 1)
                uiItem.SetItemPosition(Vector2.right * (i - 1) / 2 * (itemHeight + itemSpacingRow) + Vector2.down * (1.5f * itemSpacingCol + itemWidth));
            else
                uiItem.SetItemPosition(Vector2.right * (i / 2) * (itemHeight + itemSpacingRow) + Vector2.down * (itemSpacingCol / 2));

            uiItem.gameObject.name = "Item" + i + "-" + item.name;

            uiItem.SetCharacterName(item.name);
            uiItem.SetItemImage(item.image);
            uiItem.SetItemPrice(item.price);

            if (UserSession.Instance.userData.charactersOwned.Contains(item.name))
            {
                uiItem.SetItemAsPurchase();
                uiItem.OnItemSelect(i,item.name, OnItemSelected);

                if (UserSession.Instance.userData.characterSelected == item.name)
                {
                    uiItem.SelectItem();
                }
            }
            else
            {
                uiItem.SetItemPrice(item.price);
                uiItem.OnItemPurchase(i,item.name, item.price, OnItemPurchased);
            }
            if ((i & 1) == 0)
                ShopItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.left * (itemWidth + itemSpacingRow);
        }

    }
    void OnItemSelected(int index)
    {
        Debug.Log("select " + index);
    }
    void OnItemPurchased(int index)
    {
        Debug.Log("Purchase " + index);
    }
    void AddShopEvents()
    {
        openShopButton.onClick.RemoveAllListeners();
        openShopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.AddListener(CloseShop);
    }
    void OpenShop()
    {
        shopUI.SetActive(true);
    }
    public void CloseShop()
    {
        shopUI.SetActive(false);
    }
}
