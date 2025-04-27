using UnityEngine.UI;
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
        GenerateShopItemUI();
    }

    void GenerateShopItemUI()
    {
        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        itemWidth = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        for (int i = 0; i < itemDB.ItemCount; i++)
        {
            Item item = itemDB.GetItem(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();

            if ((i & 1) == 1)
                uiItem.SetItemPosition(Vector2.down * (i - 1) / 2 * (itemHeight + itemSpacingRow) + Vector2.right * (1.5f * itemSpacingCol + itemWidth));
            else
                uiItem.SetItemPosition(Vector2.down * (i / 2) * (itemHeight + itemSpacingRow) + Vector2.right * (itemSpacingCol / 2));

            uiItem.gameObject.name = "Item" + i + "-" + item.name;

            uiItem.SetCharacterName(item.name);
            uiItem.SetCharacterImage(item.image);
            uiItem.SetCharacterPrice(item.price);

            if (item.isPurchased)
            {
                uiItem.SetCharacterAsPurchase();
                uiItem.OnItemSelect(i, OnItemSelected);
            }
            else
            {
                uiItem.SetCharacterPrice(item.price);
                uiItem.OnItemPurchase(i, OnItemPurchased);
            }
            if ((i & 1) == 0)
                ShopItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.up * (itemHeight + itemSpacingRow);
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
