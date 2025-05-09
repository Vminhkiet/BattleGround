using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public GameObject[] tilePrefabs; // Your prefab palette (assigned in inspector)
    public HexGrid hexGrid;          // Your grid reference
    private GameObject activePrefab; // The current selected prefab

    void Awake()
    {
        SelectPrefab(0); // Default selection
    }

    void Update()
    {
        if (
            Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()
        )
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            hexGrid.ReplaceCellPrefab(hit.point, activePrefab);
        }
    }

    public void SelectPrefab(int index)
    {
        activePrefab = tilePrefabs[index];
    }
}
