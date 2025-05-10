using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Toggle, ScrollRect, Image
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental; // Required for List

public class HexMapEditor : MonoBehaviour
{
    private MapSaveLoadManager mapSaveLoadManager;
    public GameObject[] tilePrefabs;    // Your prefab palette (assigned in inspector)
    public HexGrid hexGrid;             // Your grid reference
    private GameObject activePrefab;    // The current selected prefab
    private int activePrefabIndex = 0; // Keep track of the selected index

    [Header("UI Settings")]
    public Transform scrollViewContent;      // Assign the "Content" object of your Scroll View
    public GameObject uiToggleButtonPrefab;  // Assign your PrefabToggleButton prefab
    public float previewObjectScale = 0.5f;  // Scale of the 3D preview on the button
    public Vector3 previewObjectRotation = new Vector3(0, 45, 0); // Initial rotation for the preview
    // Distance to place the preview object in front of the button's local Z, adjust as needed
    public Vector3 objectPreviewPositionOffset;

    private List<Toggle> prefabToggles = new List<Toggle>();
    private List<GameObject> previewInstances = new List<GameObject>(); // To manage preview objects

    [SerializeField]
    private EditorMode currentMode = EditorMode.Tile;

    [Header("Object placement")]
    public GameObject[] objectPrefabs;
    private GameObject activeObjectPrefab;
    private int activeObjectIndex = 0;
    public Transform objectScrollViewContent;
    private List<Toggle> objectToggles = new List<Toggle>();
    private List<GameObject> objectPreviews = new List<GameObject>();

    void Awake()
    {

        mapSaveLoadManager= GetComponent<MapSaveLoadManager>();
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogError("HexMapEditor: No tilePrefabs assigned in the inspector!");
            return;
        }

        if (scrollViewContent == null || uiToggleButtonPrefab == null)
        {
            Debug.LogError("HexMapEditor: ScrollView Content or UI Toggle Button Prefab not assigned!");
            return;
        }

        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in objectScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        if (tilePrefabs.Length > 0)
        {
            SelectPrefab(0); // Default selection (optional, can be based on UI interaction)
        }

        PrefabSelectionUIBuilder.BuildSelectionUI(
        tilePrefabs,
        scrollViewContent,
        uiToggleButtonPrefab,
        prefabToggles,
        previewInstances,
        SelectPrefab,
        objectPreviewPositionOffset,
        previewObjectRotation,
        previewObjectScale
        );

        PrefabSelectionUIBuilder.BuildSelectionUI(
        objectPrefabs,
        objectScrollViewContent,
        uiToggleButtonPrefab,
        objectToggles,
        objectPreviews,
        SelectObject,
       objectPreviewPositionOffset,
        previewObjectRotation,
        previewObjectScale
        );
    }

    public void SelectObject(int index)
    {
        if (index < 0 || index >= objectPrefabs.Length || objectPrefabs[index] == null)
        {
            Debug.LogWarning($"HexMapEditor: Attempted to select invalid object index: {index}");
            activeObjectPrefab = null;
            activeObjectIndex = -1;
            return;
        }

        activeObjectPrefab = objectPrefabs[index];
        activeObjectIndex = index;
        currentMode = EditorMode.Object;
    }

    public void SelectPrefab(int index)
    {
        if (index < 0 || index >= tilePrefabs.Length || tilePrefabs[index] == null)
        {
            Debug.LogWarning($"HexMapEditor: Attempted to select invalid prefab index: {index}");
            // Optionally deselect or handle error
            if (activePrefabIndex >= 0 && activePrefabIndex < prefabToggles.Count)
            {
                if (prefabToggles[activePrefabIndex].isOn) prefabToggles[activePrefabIndex].isOn = false;
            }
            activePrefab = null;
            activePrefabIndex = -1;
            return;
        }

        activePrefab = tilePrefabs[index];
        activePrefabIndex = index;
        // Debug.Log($"Selected prefab: {activePrefab.name} at index {index}");

        // Ensure the UI reflects this selection if it wasn't from a direct UI click
        // (e.g. initial selection in Awake)
        if (index < prefabToggles.Count && !prefabToggles[index].isOn)
        {
            prefabToggles[index].isOn = true;
        }
        currentMode = EditorMode.Tile;
    }

    public int GetTilePrefabIndex(GameObject instance)
    {
        for (int i = 0; i < tilePrefabs.Length; i++)
            if (instance.name.Contains(tilePrefabs[i].name)) return i;

        return -1;
    }

    public int GetObjectPrefabIndex(GameObject instance)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
            if (instance != null && instance.name.Contains(objectPrefabs[i].name)) return i;

        return -1;
    }


    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(inputRay, out hit))
                {
                    if (Input.GetMouseButton(0))
                    {
                        if(currentMode == EditorMode.Tile)
                            hexGrid.ReplaceCellPrefab(hit.point, activePrefab);
                        else
                            hexGrid.PlaceObjectOnTile(hit.point, activeObjectPrefab);
                }
                    else if (Input.GetKeyDown(KeyCode.C))
                    {
                        hexGrid.RotateCellTileY60(hit.point);
                    }
                }
            }
        if (Input.GetKeyDown(KeyCode.F5)) mapSaveLoadManager.SaveMap("Assets/Maps/hexmap.json");
        if (Input.GetKeyDown(KeyCode.F9)) mapSaveLoadManager.LoadMap("Assets/Maps/hexmap.json");
    }

}