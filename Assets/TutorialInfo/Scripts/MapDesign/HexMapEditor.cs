using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Toggle, ScrollRect, Image
using UnityEngine.EventSystems;
using System.Collections.Generic; // Required for List

public class HexMapEditor : MonoBehaviour
{
    public GameObject[] tilePrefabs;    // Your prefab palette (assigned in inspector)
    public HexGrid hexGrid;             // Your grid reference
    private GameObject activePrefab;    // The current selected prefab
    private int activePrefabIndex = -1; // Keep track of the selected index

    [Header("UI Settings")]
    public Transform scrollViewContent;      // Assign the "Content" object of your Scroll View
    public GameObject uiToggleButtonPrefab;  // Assign your PrefabToggleButton prefab
    public float previewObjectScale = 0.5f;  // Scale of the 3D preview on the button
    public Vector3 previewObjectRotation = new Vector3(0, 45, 0); // Initial rotation for the preview
    // Distance to place the preview object in front of the button's local Z, adjust as needed
    public float previewObjectZOffset = -1f;
    public float previewObjectYOffset = -1f;
    public float previewObjectXOffset = -1f;

    private List<Toggle> prefabToggles = new List<Toggle>();
    private List<GameObject> previewInstances = new List<GameObject>(); // To manage preview objects

    void Awake()
    {
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

        PopulatePrefabSelectionUI();

        if (tilePrefabs.Length > 0)
        {
            SelectPrefab(0); // Default selection (optional, can be based on UI interaction)
        }

    }

    void Update()
    {
        HandleInput();
    }

    void PopulatePrefabSelectionUI()
    {
        // Ensure a ToggleGroup exists on the content parent so only one toggle can be active
        ToggleGroup toggleGroup = scrollViewContent.GetComponent<ToggleGroup>();
        if (toggleGroup == null)
        {
            toggleGroup = scrollViewContent.gameObject.AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false; // Typically you want one selected
        }

        // Clear any existing previews or toggles (in case of re-population)
        foreach (GameObject preview in previewInstances)
        {
            Destroy(preview);
        }
        previewInstances.Clear();
        foreach (Toggle t in prefabToggles)
        {
            Destroy(t.gameObject);
        }
        prefabToggles.Clear();


        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            if (tilePrefabs[i] == null)
            {
                Debug.LogWarning($"HexMapEditor: Tile prefab at index {i} is null. Skipping UI element.");
                continue;
            }

            GameObject toggleButtonInstance = Instantiate(uiToggleButtonPrefab, scrollViewContent);
            toggleButtonInstance.name = $"PrefabToggle_{tilePrefabs[i].name}";

            Toggle toggle = toggleButtonInstance.GetComponent<Toggle>();
            if (toggle == null)
            {
                Debug.LogError($"The UI Toggle Button Prefab '{uiToggleButtonPrefab.name}' is missing a Toggle component!");
                Destroy(toggleButtonInstance);
                continue;
            }

            toggle.group = toggleGroup;
            prefabToggles.Add(toggle);

            // Capture the index for the listener
            int currentIndex = i;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SelectPrefab(currentIndex);
                }
            });

            // --- Create and position the 3D preview object ---
            GameObject previewInstance = Instantiate(tilePrefabs[i]);
            previewInstances.Add(previewInstance);

            // Parent the preview to the button instance.
            // It's crucial to parent it to a transform within the button that doesn't get scaled weirdly by layouts.
            // Often, a dedicated child GameObject on the button prefab for holding the preview is best.
            // For simplicity here, we'll parent directly to the toggle button instance's transform.
            previewInstance.transform.SetParent(toggleButtonInstance.transform, false); // Set worldPositionStays to false

            // Remove physics components from the preview to prevent interactions
            RemovePhysics(previewInstance);

            // Set a layer for previews if you want a separate camera to render them
            // or to exclude them from main camera raycasts for gameplay.
            // SetLayerRecursively(previewInstance, LayerMask.NameToLayer("UIPreviewLayer")); // Example

            // Adjust transform for preview
            previewInstance.transform.localPosition = new Vector3(previewObjectXOffset, previewObjectYOffset, previewObjectZOffset); // Center it, adjust Z for visibility
            previewInstance.transform.localRotation = Quaternion.Euler(previewObjectRotation);
            previewInstance.transform.localScale = Vector3.one * previewObjectScale;

            // Ensure the preview itself doesn't block UI raycasts meant for the toggle
            SetRaycastTargetable(previewInstance, false);
        }
    }

    void RemovePhysics(GameObject obj)
    {
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
        {
            Destroy(c);
        }
        foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>())
        {
            Destroy(rb);
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (null == child) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void SetRaycastTargetable(GameObject obj, bool targetable)
    {
        // If the object has Graphic components (e.g., Image, RawImage), set their raycastTarget
        Graphic[] graphics = obj.GetComponentsInChildren<Graphic>();
        foreach (Graphic g in graphics)
        {
            g.raycastTarget = targetable;
        }

        // For 3D models, ensuring they don't have colliders (done in RemovePhysics)
        // is usually the main step to prevent them from blocking UI raycasts.
        // If they still block, ensure your actual UI Toggle (which has a Graphic for raycasting)
        // is effectively "in front" or the 3D model is on a layer ignored by the UI's GraphicRaycaster.
    }

    void HandleInput()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; 
            if (Input.GetMouseButton(0))
            {
                if (activePrefab == null)
                {
                    // Debug.Log("No active prefab selected to place.");
                    return;
                }
                if (Physics.Raycast(inputRay, out hit)) // Ensure you're raycasting against your hex grid layer if needed
                {
                    hexGrid.ReplaceCellPrefab(hit.point, activePrefab);
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (Physics.Raycast(inputRay, out hit)) 
                {
                    hexGrid.RotateCellTileY60(hit.point);
                }
            }

        }
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
    }
}