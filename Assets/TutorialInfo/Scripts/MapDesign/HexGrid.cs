using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 20;
    public int height = 20;

    public HexCell cellPrefab;
    public Text cellLabelPrefab; // Keep if you might use it later

    public GameObject defaultTilePrefab; // Tile prefab placed on each cell when first created

    HexCell[] cells;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        // Assuming HexMetrics.innerRadius and HexMetrics.outerRadius are defined elsewhere
        // For example, in a static class HexMetrics
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z); // Assuming HexCoordinates is defined

        // Instantiate the default tile prefab and parent it to the cell
        if (defaultTilePrefab != null)
        {
            // Instantiate with the cell's world position and identity rotation, then parent
            GameObject tile = Instantiate(defaultTilePrefab, cell.transform.position, Quaternion.identity);
            tile.transform.SetParent(cell.transform); // Parent to the cell's transform
            cell.currentTile = tile;
        }

        // Create coordinate label (optional)
        /* Text label = Instantiate<Text>(cellLabelPrefab);
            if (gridCanvas != null && label != null) // Add null checks
            {
                label.rectTransform.SetParent(gridCanvas.transform, false);
                label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
                label.text = cell.coordinates.ToStringOnSeparateLines();
            }
        */
    }

    public void ReplaceCellPrefab(Vector3 worldHitPosition, GameObject newPrefab)
    {
        // Convert the world hit position to local grid space to find coordinates
        Vector3 localHitPosition = transform.InverseTransformPoint(worldHitPosition);
        HexCoordinates coordinates = HexCoordinates.FromPosition(localHitPosition); // Assuming this method exists

        // This index calculation is typical for offset coordinate systems (like in Catlike Coding's tutorials)
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        HexCell cell = cells[index];
        if (cell == null)
        {
            Debug.LogWarning($"ReplaceCellPrefab: Cell at index {index} is null.");
            return;
        }

        // Remove current tile GameObject
        if (cell.currentTile != null)
        {
#if UNITY_EDITOR
            // Using DestroyImmediate in editor mode if this is called from an editor script
            // or in response to an immediate action where Destroy might be too slow.
            if (!Application.isPlaying)
            {
                DestroyImmediate(cell.currentTile);
            }
            else
            {
                Destroy(cell.currentTile);
            }
#else
            Destroy(cell.currentTile);
#endif
        }

        // Instantiate and assign new prefab
        if (newPrefab != null)
        {
            // Instantiate at cell's world position, with identity rotation, then parent
            GameObject newTile = Instantiate(newPrefab, cell.transform.position, Quaternion.identity);
            newTile.transform.SetParent(cell.transform); // Parent to the cell's transform
            cell.currentTile = newTile;
        }
        else
        {
            cell.currentTile = null; // Ensure currentTile is null if newPrefab is null
        }
    }

    /// <summary>
    /// Rotates the current tile of the cell at the given world position by 90 degrees around its local Y-axis.
    /// </summary>
    /// <param name="worldHitPosition">The world position to identify the cell (e.g., from a raycast hit).</param>
    public void RotateCellTileY60(Vector3 worldHitPosition)
    {
        // Convert the world hit position to local grid space to find coordinates
        Vector3 localHitPosition = transform.InverseTransformPoint(worldHitPosition);
        HexCoordinates coordinates = HexCoordinates.FromPosition(localHitPosition); // Assuming this method exists

        // This index calculation is typical for offset coordinate systems
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        if (index < 0 || index >= cells.Length)
        {
            Debug.LogWarning($"RotateCellTileY90: Index {index} out of bounds for coordinates {coordinates}. Position: {worldHitPosition}");
            return;
        }

        HexCell cell = cells[index];
        if (cell == null)
        {
            Debug.LogWarning($"RotateCellTileY90: Cell at index {index} (coords: {coordinates}) is null.");
            return;
        }

        if (cell.currentTile != null)
        {
            cell.currentTile.transform.Rotate(0f, 60f, 0f, Space.Self);
        }
        else
        {
            Debug.Log($"RotateCellTileY60: Cell at {coordinates} has no current tile to rotate.");
        }
    }

    public void PlaceObjectOnTile(Vector3 position, GameObject objectPrefab)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        if (index < 0 || index >= cells.Length) return;

        HexCell cell = cells[index];

        // Optional: clear existing decoration
        if (cell.decorationObject != null)
        {
            DestroyImmediate(cell.decorationObject);
        }

        Vector3 spawnPos = cell.transform.position + new Vector3(0, 0.5f, 0);
        cell.decorationObject = Instantiate(objectPrefab, spawnPos, Quaternion.identity, cell.transform);
    }
}
