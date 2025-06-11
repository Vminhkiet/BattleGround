using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Toggle, ScrollRect, Image
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;
using UnityEditorInternal; // Required for List

public class HexMapEditor : MonoBehaviour
{
    public MapObjectDatabase m_MapObjectDatabase;
    public HexGrid hexGrid;        
    private GameObject activePrefab; 
    private int activePrefabIndex = 0; 

    [Header("UI Settings")]
    public Transform scrollViewContent;     
    public GameObject uiToggleButtonPrefab;  
    public float previewObjectScale = 0.5f;  
    public Vector3 previewObjectRotation = new Vector3(0, 45, 0); 
    public Vector3 objectPreviewPositionOffset;

    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;
    private GameObject lastRotatedObject; 
    private HexCell lastRotatedCell; 

    private List<Toggle> prefabToggles = new List<Toggle>();
    private List<GameObject> previewInstances = new List<GameObject>(); 

    [SerializeField]
    private EditorMode currentMode = EditorMode.Tile;

    [Header("Object placement")]
    private GameObject activeObjectPrefab;
    private int activeObjectIndex = 0;
    public Transform objectScrollViewContent;
    private List<Toggle> objectToggles = new List<Toggle>();
    private List<GameObject> objectPreviews = new List<GameObject>();

    void Awake()
    {
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

        SelectPrefab(0);

        PrefabSelectionUIBuilder.BuildSelectionUI(
        m_MapObjectDatabase.tilePrefabs,
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
         m_MapObjectDatabase.objectPrefabs,
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
        if (index < 0 || index >= m_MapObjectDatabase.objectCount || m_MapObjectDatabase.GetObject(index) == null)
        {
            Debug.LogWarning($"HexMapEditor: Attempted to select invalid object index: {index}");
            activeObjectPrefab = null;
            activeObjectIndex = -1;
            return;
        }

        activeObjectPrefab = m_MapObjectDatabase.GetObject(index);
        activeObjectIndex = index;
        currentMode = EditorMode.Object;
    }

    public void SelectPrefab(int index)
    {
        if (index < 0 || index >= m_MapObjectDatabase.tileCount || m_MapObjectDatabase.GetTile(index) == null)
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

        activePrefab = m_MapObjectDatabase.GetTile(index);
        activePrefabIndex = index;
        if (index < prefabToggles.Count && !prefabToggles[index].isOn)
        {
            prefabToggles[index].isOn = true;
        }
        currentMode = EditorMode.Tile;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            // Đặt Tile
            if (Input.GetMouseButton(0) && currentMode == EditorMode.Tile)
            {
                if (activePrefab != null)
                {
                    hexGrid.ReplaceCellPrefab(hit.point, activePrefab);
                }
                else
                {
                    Debug.LogWarning("No active tile prefab selected.");
                }
            }
            // Đặt Object
            else if (Input.GetMouseButtonDown(0) && currentMode == EditorMode.Object)
            {
                if (m_MapObjectDatabase.objectCount > activeObjectIndex && activeObjectIndex >= 0)
                {
                    hexGrid.PlaceObjectOnTile(hit.point, m_MapObjectDatabase.GetObject(activeObjectIndex));
                }
                else
                {
                    Debug.LogWarning("No active object prefab selected or index out of bounds.");
                }
            }

            // Handle continuous rotation while holding R
            if (Input.GetKey(KeyCode.R))
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;

                if (currentMode == EditorMode.Tile)
                {
                    // Get the cell at hit point
                    Vector3 localHitPosition = hexGrid.transform.InverseTransformPoint(hit.point);
                    HexCoordinates coordinates = HexCoordinates.FromPosition(localHitPosition);
                    int index = coordinates.X + coordinates.Z * hexGrid.width + coordinates.Z / 2;

                    if (index >= 0 && index < hexGrid.GetAllCells().Length)
                    {
                        HexCell cell = hexGrid.GetAllCells()[index];
                        if (cell != null && cell.currentTile != null)
                        {
                            // Only rotate if we're still hovering over the same cell
                            if (lastRotatedCell != cell)
                            {
                                lastRotatedCell = cell;
                            }
                            cell.currentTile.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
                        }
                    }
                }
                else if (currentMode == EditorMode.Object)
                {
                    if (hit.collider != null && hit.collider.gameObject.CompareTag("DecorationObject"))
                    {
                        GameObject objectToRotate = hit.collider.gameObject;
                        // Only rotate if we're still hovering over the same object
                        if (lastRotatedObject != objectToRotate)
                        {
                            lastRotatedObject = objectToRotate;
                        }
                        objectToRotate.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
                    }
                }
            }
            else
            {
                // Reset tracking when R is released
                lastRotatedObject = null;
                lastRotatedCell = null;
            }

            // Handle deletion
            if (Input.GetKey(KeyCode.Backspace))
            {
                if (hit.collider != null)
                {
                    if (currentMode == EditorMode.Object && hit.collider.gameObject.CompareTag("DecorationObject"))
                    {
                        GameObject decorationObject = hit.collider.gameObject;
                        Destroy(decorationObject);
                    }
                    else if (currentMode == EditorMode.Tile)
                    {

                        Vector3 localHitPosition = hexGrid.transform.InverseTransformPoint(hit.point);
                        HexCoordinates coordinates = HexCoordinates.FromPosition(localHitPosition);
                        int index = coordinates.X + coordinates.Z * hexGrid.width + coordinates.Z / 2;

                        if (index >= 0 && index < hexGrid.GetAllCells().Length)
                        {
                            HexCell cell = hexGrid.GetAllCells()[index];
                            if (cell != null && cell.currentTile != null)
                            {
                                hexGrid.ReplaceCellPrefab(hit.point, null);
                            }
                        }
                    }
                }
            }
        }
    }

}