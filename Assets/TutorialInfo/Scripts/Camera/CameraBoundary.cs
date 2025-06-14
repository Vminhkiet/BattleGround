using UnityEngine;

public class CameraBoundary : MonoBehaviour
{
    [Tooltip("The target for the camera to follow (the player/character).")]
    public Transform target;

    [Tooltip("The HexGrid component in the scene to define the map boundaries.")]
    public HexGrid hexGrid;

    [Tooltip("The offset from the target's position.")]
    public Vector3 offset = new Vector3(0f, 10f, -5f);
    
    // The camera's rotation is fixed.
    private readonly Quaternion cameraRotation = Quaternion.Euler(45, 0, 0);

    private float minX, maxX, minZ, maxZ;
    private bool boundariesCalculated = false;

    void Start()
    {
        if (target == null || hexGrid == null)
        {
            Debug.LogError("CameraBoundary: Target or HexGrid not assigned in the Inspector!");
            enabled = false; // Disable this script if setup is incomplete.
            return;
        }

        CalculateMapBoundaries();
        transform.rotation = cameraRotation;
    }

    void LateUpdate()
    {
        if (!boundariesCalculated || target == null) return;

        // Calculate the desired camera position based on the target and offset.
        Vector3 desiredPosition = target.position + offset;

        // Clamp the camera's X and Z position to the map boundaries.
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedZ = Mathf.Clamp(desiredPosition.z, minZ, maxZ);

        // Update the camera's position. The Y position is determined by the offset.
        transform.position = new Vector3(clampedX, desiredPosition.y, clampedZ);
    }

    /// <summary>
    /// Calculates the world-space boundaries of the hex grid.
    /// </summary>
    void CalculateMapBoundaries()
    {
        float mapWidth = hexGrid.width;
        float mapHeight = hexGrid.height;

        // Get hex size constants.
        float hexOuterRadius = HexMetrics.outerRadius;
        float hexInnerRadius = HexMetrics.innerRadius;
        
        // The grid is generated with an offset coordinate system ("odd-q" vertical layout).
        // The local X position of a hex center is: (col + row_offset) * hex_width
        // where row_offset is 0 for even rows and 0.5 for odd rows.
        // The local Z position is: row * hex_height_step
        
        // The widest part of the grid in local space.
        // Min local X is at column 0 on an even row (offset 0).
        // Max local X is at the last column on an odd row (offset 0.5).
        float minLocalX = 0f;
        float maxLocalX = (mapWidth - 1 + 0.5f) * (hexInnerRadius * 2f);
        
        float minLocalZ = 0f;
        float maxLocalZ = (mapHeight - 1) * (hexOuterRadius * 1.5f);
        
        // Create vectors for the min and max local points of the hex centers' bounding box.
        Vector3 localMin = new Vector3(minLocalX, 0, minLocalZ);
        Vector3 localMax = new Vector3(maxLocalX, 0, maxLocalZ);

        // Transform these local points to world space using the HexGrid's transform.
        // This correctly handles the grid's position, rotation, and scale in the world.
        Vector3 worldMin = hexGrid.transform.TransformPoint(localMin);
        Vector3 worldMax = hexGrid.transform.TransformPoint(localMax);

        // Determine the final min/max coordinates for the bounding box.
        // Using Min/Max ensures it works even if the grid is rotated.
        minX = Mathf.Min(worldMin.x, worldMax.x);
        maxX = Mathf.Max(worldMin.x, worldMax.x);
        minZ = Mathf.Min(worldMin.z, worldMax.z);
        maxZ = Mathf.Max(worldMin.z, worldMax.z);

        // Add a padding equal to the hex radius. This ensures the camera view doesn't
        // slightly go over the edge, showing the empty space just beyond the last hex tile.
        minX -= hexOuterRadius;
        maxX += hexOuterRadius;
        minZ -= hexOuterRadius;
        maxZ += hexOuterRadius;
        
        boundariesCalculated = true;
        Debug.Log($"Map Boundaries Calculated: MinX={minX}, MaxX={maxX}, MinZ={minZ}, MaxZ={maxZ}");
    }
} 