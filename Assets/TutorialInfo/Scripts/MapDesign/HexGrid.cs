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

        if (index < 0 || index >= cells.Length)
        {
            Debug.LogWarning($"ReplaceCellPrefab: Index {index} out of bounds for coordinates {coordinates}. Position: {worldHitPosition}");
            return;
        }

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
            // Rotate the currentTile's transform by 90 degrees around its own local Y-axis.
            cell.currentTile.transform.Rotate(0f, 60f, 0f, Space.Self);
            // You could also use: cell.currentTile.transform.Rotate(Vector3.up * 90f, Space.Self);
        }
        else
        {
            Debug.Log($"RotateCellTileY60: Cell at {coordinates} has no current tile to rotate.");
        }
    }
}

// You would also need definitions for HexCell, HexCoordinates, and HexMetrics.
// Example stubs (replace with your actual implementations):

/*
public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public GameObject currentTile;
    // Add other cell properties here
}

public static class HexMetrics
{
    public const float outerRadius = 10f; // Example value
    public const float innerRadius = outerRadius * 0.866025404f; // Sqrt(3)/2
    // Add other metrics
}

[System.Serializable]
public struct HexCoordinates
{
    public int X { get; private set; }
    public int Z { get; private set; }

    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z); // Example for "odd-r" or "even-r" offset
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        // This conversion is more complex and depends heavily on your hex grid math.
        // The following is a simplified placeholder from Catlike Coding's tutorial style.
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;
        float offset = position.z / (HexMetrics.outerRadius * 1.5f);
        x -= offset * 0.5f;
        y -= offset * 0.5f;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0) {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x -y - iZ);

            if (dX > dY && dX > dZ) {
                iX = -iY - iZ;
            }
            else if (dZ > dY) {
                iZ = -iX - iY;
            }
        }
        // This returns axial coordinates, your index calculation might expect offset.
        // If your index needs offset X directly:
        // For "odd-r" where X is `x - z / 2` in axial, then to get back to that offset X:
        // int offsetCoordinateX = iX + iZ / 2;
        // return new HexCoordinates(offsetCoordinateX, iZ);
        // Ensure this matches the coordinate system your index relies on.
        // For simplicity, if FromPosition is expected to return the same kind of X, Z as FromOffsetCoordinates stores:
        return new HexCoordinates(iX + iZ / 2, iZ); // This converts axial (iX, iZ) back to an offset X for indexing
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Z.ToString();
	}
}
*/