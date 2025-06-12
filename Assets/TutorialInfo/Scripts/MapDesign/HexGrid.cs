using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class HexGrid : MonoBehaviour
{
    public int width = 1000;
    public int height = 1000;

    public HexCell cellPrefab;
    public Text cellLabelPrefab; // Keep if you might use it later

    public GameObject defaultTilePrefab; // Tile prefab placed on each cell when first created

    Dictionary<Vector2Int, HexCell> cells;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new Dictionary<Vector2Int, HexCell>();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
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

        cells = new Dictionary<Vector2Int, HexCell>();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z);
            }
        }
    }

    void CreateCell(int x, int z)
    {
        Vector3 position;
        // Assuming HexMetrics.innerRadius and HexMetrics.outerRadius are defined elsewhere
        // For example, in a static class HexMetrics
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z); // Assuming HexCoordinates is defined
        cells[new Vector2Int(x, z)] = cell;

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
        Vector2Int key = new Vector2Int(coordinates.X, coordinates.Z);

        if (!cells.TryGetValue(key, out HexCell cell))
        {
            Debug.LogWarning($"ReplaceCellPrefab: Cell at coordinates {key.x},{key.y} does not exist.");
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
        
    public GameObject PlaceObjectOnTile(Vector3 position, GameObject objectPrefab)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Vector2Int key = new Vector2Int(coordinates.X, coordinates.Z);

        if (!cells.TryGetValue(key, out HexCell cell))
        {
            return null;
        }

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
        // Convert cube coordinates (x, z) to offset coordinates for position calculation.
        // The grid generation and cell positioning logic in CreateCell uses offset coordinates.
        int offsetX = x + (z / 2);
        int offsetZ = z;

        Vector3 position;
        position.x = (offsetX + offsetZ * 0.5f - offsetZ / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = offsetZ * (HexMetrics.outerRadius * 1.5f);

        return transform.TransformPoint(position); // Convert local to world position
    }

    public HexCell[] GetAllCells()
    {
        return cells.Values.ToArray();
    }
    public HexCell GetCellAtCoordinates(int x, int z)
    {
        cells.TryGetValue(new Vector2Int(x, z), out HexCell cell);
        return cell;
    }

    public HexCell GetOrCreateCellAt(int x, int z)
    {
        Vector2Int coords = new Vector2Int(x, z);
        if (cells.TryGetValue(coords, out HexCell cell))
        {
           if (cell != null)
            return cell;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("cellPrefab is not assigned in HexGrid.");
            return null;
        }

        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        cell = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        cells[coords] = cell;

        return cell;
    }

    public void ClearAllCells(bool isEditorMode = false)
    {
        if (cells == null) // 'cells' là mảng/list chứa tất cả HexCell của bạn
        {
            return;
        }

        foreach (HexCell cell in cells.Values) // cells là nơi bạn lưu trữ tất cả các HexCell
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
        }
        Debug.Log("Tất cả các ô đã được dọn dẹp.");
#if UNITY_EDITOR
        if (isEditorMode) EditorUtility.SetDirty(this); // Đánh dấu HexGrid là dirty
#endif
    }
}
