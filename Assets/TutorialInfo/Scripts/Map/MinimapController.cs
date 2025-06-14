using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Settings")]
    public Camera minimapCamera;
    public RawImage minimapDisplay;
    public float minimapHeight = 50f;
    public float minimapSize = 200f;
    public float zoomLevel = 20f;
    
    [Header("References")]
    public Transform playerTransform;
    public HexGrid hexGrid;

    private RenderTexture minimapTexture;
    private Vector3 lastPlayerPosition;
    private bool needsUpdate = true;

    [Header("Minimap Icon")]
    public GameObject minimapDotPrefab;
    private GameObject minimapDotInstance;
    private Quaternion iconBaseRotation;

    void Start()
    {
        if (minimapCamera == null || minimapDisplay == null)
        {
            Debug.LogError("Minimap camera or display not assigned!");
            enabled = false;
            return;
        }
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;

        // Create render texture for minimap
        minimapTexture = new RenderTexture((int)minimapSize, (int)minimapSize, 16, RenderTextureFormat.ARGB32);
        minimapTexture.Create();

        // Set up minimap camera
        minimapCamera.targetTexture = minimapTexture;
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = zoomLevel;
        minimapDisplay.texture = minimapTexture;

        // Position camera above the map
        if (playerTransform != null)
        {
            UpdateMinimapCameraPosition();
        }

        if (playerTransform != null && minimapDotPrefab != null)
        {
            minimapDotInstance = Instantiate(minimapDotPrefab);
            minimapDotInstance.layer = LayerMask.NameToLayer("MiniMapOnly");
            iconBaseRotation = minimapDotInstance.transform.rotation;
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Only update if player has moved significantly
        if (Vector3.Distance(lastPlayerPosition, playerTransform.position) > 1f || needsUpdate)
        {
            UpdateMinimapCameraPosition();
            lastPlayerPosition = playerTransform.position;
            needsUpdate = false;
        }

        if (minimapDotInstance != null && playerTransform != null)
        {
            // Position it
            Vector3 iconPos = playerTransform.position;
            iconPos.y += 0.5f;
            minimapDotInstance.transform.position = iconPos;

            // Get the player's yaw
            float yaw = playerTransform.eulerAngles.y;

            // Apply rotation relative to base
            minimapDotInstance.transform.rotation = iconBaseRotation * Quaternion.Euler(0f, 0f, -yaw);
        }

    }

    void UpdateMinimapCameraPosition()
    {
        if (playerTransform == null) return;

        // Position camera above player
        Vector3 newPosition = playerTransform.position;
        newPosition.y = minimapHeight;
        minimapCamera.transform.position = newPosition;

        // Make camera look straight down
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void OnDestroy()
    {
        if (minimapTexture != null)
        {
            minimapTexture.Release();
            Destroy(minimapTexture);
        }
    }

    // Call this when the map changes or when you need to force a minimap update
    public void ForceUpdate()
    {
        needsUpdate = true;
    }
} 