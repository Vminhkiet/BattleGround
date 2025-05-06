using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

class ItemUI : MonoBehaviour
{

    [SerializeField] Color useBtnColor;
    [SerializeField] Color purchaseBtnColor;

    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemPriceText;
    [SerializeField] Button itemPurchaseButton;

    [Space(20f)]
    [SerializeField] Button itemButton;
    [SerializeField] Image freeBanner;
    [SerializeField] Image coinImage;
    [SerializeField] Image itemImage;
    [SerializeField] Outline itemOutline;

    private bool isItemPurchased;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }

    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }

    public void SetCharacterName(string name)
    {
        itemNameText.text = name;
    }

    public void SetItemPrice(int price)
    {
        itemPriceText.text = price.ToString();
        ColorBlock cb = itemPurchaseButton.colors;
        if (price > 0)
        {
            freeBanner.gameObject.SetActive(false);
            cb.normalColor = purchaseBtnColor;
            itemPurchaseButton.colors = cb;
        }
        else
        {
            itemPriceText.text = "USE";
            SetItemAsPurchase();
        }
    }
    public void SetItemAsPurchase()
    {
        itemPriceText.text = "USE";
        ColorBlock cb = itemPurchaseButton.colors;
        cb.normalColor = useBtnColor;
        itemPurchaseButton.colors = cb;
        coinImage.gameObject.SetActive(false);
    }

    public void OnItemPurchase(int itemIndex, UnityAction<int> action)
    {
        itemPurchaseButton.onClick.RemoveAllListeners();
        itemPurchaseButton.onClick.AddListener(() => action.Invoke(itemIndex));
    }
    public void OnItemSelect(int itemIndex, UnityAction<int> action)
    {
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() => action.Invoke(itemIndex));
        itemPurchaseButton.onClick.AddListener(SelectItem);
        CalculatorController.instance.infoplayer.selectedItem = itemNameText.text;
      
    }

    public void SelectItem()
    {
        itemOutline.enabled = true;
        itemPriceText.text = "Using";
    }

    public void DeselectItem()
    {
        itemOutline.enabled = false;
    }
}
