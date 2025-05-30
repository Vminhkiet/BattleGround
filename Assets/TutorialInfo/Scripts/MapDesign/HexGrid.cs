using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HexGrid : MonoBehaviour
{
    public int width = 1000;
    public int height = 1000;

    public HexCell cellPrefab;
    public Text cellLabelPrefab; // Keep if you might use it later

    public GameObject defaultTilePrefab; // Tile prefab placed on each cell when first created

    HexCell[] cells;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[height * width];
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    public void BuildGrid()
    {
#if UNITY_EDITOR
        // Clear previous cells
        foreach (Transform child in transform)
        {
            if (Application.isEditor)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
        }
#endif

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
    }

    public GameObject ReplaceCellPrefab(Vector3 worldHitPosition, GameObject newPrefab)
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
            return null;
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
            return newTile;
        }
        else
        {
            cell.currentTile = null; // Ensure currentTile is null if newPrefab is null
            return null;
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
        
    public GameObject PlaceObjectOnTile(Vector3 position, GameObject objectPrefab)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        if (index < 0 || index >= cells.Length) return null;

        HexCell cell = cells[index];

        // Optional: clear existing decoration
        if (cell.decorationObject != null)
        {
            DestroyImmediate(cell.decorationObject);
        }

        Vector3 spawnPos = cell.transform.position+new Vector3(0,-0.1f,0);
        cell.decorationObject = Instantiate(objectPrefab, spawnPos, Quaternion.identity, cell.transform);
        return cell.decorationObject;
    }

    public Vector3 GetWorldPositionFromCoordinates(int x, int z)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        return transform.TransformPoint(position); // Convert local to world position
    }

    public HexCell[] GetAllCells()
    {
        return cells;
    }
    public HexCell GetCellAtCoordinates(int x, int z)
    {
        int index = x + z * width + z / 2;
        if (index < 0 || index >= cells.Length)
            return null;
        return cells[index];
    }

    public void ClearAllCells(bool isEditorMode = false)
    {
        if (cells == null) // 'cells' là mảng/list chứa tất cả HexCell của bạn
        {
            Debug.LogWarning("Mảng 'cells' trong HexGrid chưa được khởi tạo hoặc null.");
            return;
        }

        foreach (HexCell cell in cells) // cells là nơi bạn lưu trữ tất cả các HexCell
        {
            if (cell == null) continue;
            if (cell.currentTile != null)
            {
                if (isEditorMode) DestroyImmediate(cell.currentTile); else Destroy(cell.currentTile);
                cell.currentTile = null;
            }
            if (cell.decorationObject != null)
            {
                if (isEditorMode) DestroyImmediate(cell.decorationObject); else Destroy(cell.decorationObject);
                cell.decorationObject = null;
            }
            // Reset các thuộc tính khác của cell nếu cần
        }
        Debug.Log("Tất cả các ô đã được dọn dẹp.");
#if UNITY_EDITOR
        if (isEditorMode) EditorUtility.SetDirty(this); // Đánh dấu HexGrid là dirty
#endif
    }
}
