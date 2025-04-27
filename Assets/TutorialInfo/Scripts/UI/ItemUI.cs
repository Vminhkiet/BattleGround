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

    [SerializeField] Color itemNotSelectedColor;
    [SerializeField] Color itemSelectedColor;

    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemPriceText;
    [SerializeField] Button itemPurchaseButton;

    [Space(20f)]
    [SerializeField] Button itemButton;
    [SerializeField] Image freeBanner;
    [SerializeField] Image itemImage;
    [SerializeField] Outline itemOutline;

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
        if(price > 0)
            freeBanner.gameObject.SetActive(false);
    }
    public void SetItemAsPurchase()
    {
        itemButton.interactable = true;
        itemImage.color = itemNotSelectedColor;
        itemPriceText.text = "USE";
    }

    public void OnItemPurchase(int itemIndex, UnityAction<int> action)
    {
        itemPurchaseButton.onClick.RemoveAllListeners();
        itemPurchaseButton.onClick.AddListener(() => action.Invoke(itemIndex));

        itemImage.color = itemNotSelectedColor;
    }
    public void OnItemSelect(int itemIndex, UnityAction<int> action)
    {
        itemButton.interactable = true;
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() => action.Invoke(itemIndex));

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
