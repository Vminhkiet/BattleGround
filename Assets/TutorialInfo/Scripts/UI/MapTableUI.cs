using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
public class MapTableUI : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] float itemSpacingCol;
    float itemWidth;

    [Header("UI elements")]
    [SerializeField] Transform MapItemsContainer;
    [SerializeField] GameObject itemPrefab;
    [Space(20)]
    [SerializeField] MapDatabase mapDatabase;
    
    [Header("Shop Events")]
    [SerializeField] GameObject mapUI;
    [SerializeField] Button openMapButton;
    [SerializeField] Button closeMapButton;

    // Start is called before the first frame update
    void Start()
    {
        AddShopEvents();
        GeneratorMapItemUI();
    }

    public void GeneratorMapItemUI() {
        itemWidth = MapItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        Destroy(MapItemsContainer.GetChild(0).gameObject);

        for (int i = 0; i < mapDatabase.MapCount; i++)
        {
            Map map = mapDatabase.GetMap(i);
            MapItemUI mapItem = Instantiate(itemPrefab, MapItemsContainer).GetComponent<MapItemUI>();

            mapItem.SetItemPosition(Vector2.right * i * (itemWidth + itemSpacingCol));

            mapItem.SetMapImage(map.sprite);
            mapItem.SetMapName(map.name);
            //mapItem.OnItemSelect(i, OnItemSelected);
            MapItemsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.up * (itemWidth + itemSpacingCol);
        }
    }

    void AddShopEvents()
    {
        openMapButton.onClick.RemoveAllListeners();
        openMapButton.onClick.AddListener(OpenUI);
    }

    void OnItemSelected(int index)
    {
        Debug.Log("select " + index);
    }
    public void OpenUI()
    {
        mapUI.SetActive(true);
    }
    public void CloseUI()
    {
        mapUI.SetActive(false);
    }
}
