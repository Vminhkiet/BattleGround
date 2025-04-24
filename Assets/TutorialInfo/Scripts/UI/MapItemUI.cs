using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class MapItemUI : MonoBehaviour
{
    [SerializeField] Color itemNotSelectedColor;
    [SerializeField] Color itemSelectedColor;

    [SerializeField] Image mapImage;
    [SerializeField] TMP_Text mapName;

    [Space(20f)]
    [SerializeField] Button itemButton;
    [SerializeField] Image itemImage;
    [SerializeField] Outline itemOutline;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }
    public void SetMapImage(Sprite sprite)
    {
        mapImage.sprite = sprite;
    }
    public void SetMapName(string newName)
    {
        mapName.text = newName;
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
