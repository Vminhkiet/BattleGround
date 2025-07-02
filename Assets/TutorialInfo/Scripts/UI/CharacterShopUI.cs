using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterShopUI : MonoBehaviour
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
    [SerializeField] CharacterShopDatabase characterDB;
  

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
        Destroy(ShopItemsContainer.GetChild (0).gameObject);
        for (int i = 0; i < characterDB.CharactersCount; i++)
        {
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem =  Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();

            if ((i & 1) == 1)
                uiItem.SetItemPosition(Vector2.down * (i - 1) / 2 * (itemHeight + itemSpacingRow) + Vector2.right * (1.5f * itemSpacingCol + itemWidth));
            else 
                uiItem.SetItemPosition(Vector2.down * (i / 2) * (itemHeight + itemSpacingRow) + Vector2.right * (itemSpacingCol / 2));

            uiItem.gameObject.name = "Item" + i + "-" + character.name;

            uiItem.SetCharacterHealth(character.health);
            uiItem.SetCharacterName(character.name);
            uiItem.SetCharacterImage(character.image);
            uiItem.SetCharacterPower(character.power);
            uiItem.SetCharacterPrice(character.price);
            uiItem.SetCharacterSpeed(character.speed);

            if (UserSession.Instance.userData.charactersOwned.Contains(character.name))
            {
                uiItem.SetCharacterAsPurchase();
                uiItem.OnItemSelect(i, character.name, OnItemSelected);

                if (UserSession.Instance.userData.characterSelected == character.name)
                {
                    uiItem.SelectItem(); 
                }
            }
            else
            {
                uiItem.SetCharacterPrice(character.price);
                uiItem.OnItemPurchase(i, character.price, character.type.ToString(), OnItemPurchased);
            }
            if ((i & 1) == 0)
                ShopItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.up * (itemHeight + itemSpacingRow)*2;
        }
        
    }
    void OnItemSelected(int index) {
        CloseShop();
    }
    void OnItemPurchased(int index)
    {

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
