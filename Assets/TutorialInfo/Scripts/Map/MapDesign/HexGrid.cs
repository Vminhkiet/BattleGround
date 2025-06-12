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
        // It's safer to check for Application.isPlaying to avoid issues in prefab mode, etc.
        if (Application.isPlaying)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public void BuildGrid()
    {
#if UNITY_EDITOR
        // Clear previous cells. Use DestroyImmediate in editor.
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
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
        // This function creates a cell using OFFSET coordinates (x, z)
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        
        // Convert offset coordinates to axial and store them
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        
        // Use AXIAL coordinates for the dictionary key.
        Vector2Int axialCoords = new Vector2Int(cell.coordinates.X, cell.coordinates.Z);
        cells[axialCoords] = cell;

        // Instantiate the default tile prefab and parent it to the cell
        if (defaultTilePrefab != null)
        {
            GameObject tile = Instantiate(defaultTilePrefab, cell.transform.position, Quaternion.identity);
            tile.transform.SetParent(cell.transform);
            cell.currentTile = tile;
        }
    }

    public GameObject ReplaceCellPrefab(Vector3 worldHitPosition, GameObject newPrefab)
    {
        Vector3 localHitPosition = transform.InverseTransformPoint(worldHitPosition);
        HexCoordinates coordinates = HexCoordinates.FromPosition(localHitPosition); 
        
        // Use AXIAL coordinates for lookup
        Vector2Int key = new Vector2Int(coordinates.X, coordinates.Z);

        if (!cells.TryGetValue(key, out HexCell cell))
        {
            Debug.LogWarning($"ReplaceCellPrefab: Cell at coordinates {key.x},{key.y} does not exist.");
            return null;
        }

        if (cell.currentTile != null)
        {
            if (Application.isPlaying)
                Destroy(cell.currentTile);
            else
                DestroyImmediate(cell.currentTile);
        }

        if (newPrefab != null)
        {
            GameObject newTile = Instantiate(newPrefab, cell.transform.position, Quaternion.identity);
            newTile.transform.SetParent(cell.transform); 
            cell.currentTile = newTile;
            return newTile;
        }
        else
        {
            cell.currentTile = null; 
            return null;
        }
    } 
        
    public GameObject PlaceObjectOnTile(Vector3 worldPosition, GameObject objectPrefab)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        HexCoordinates coordinates = HexCoordinates.FromPosition(localPosition);
        
        // Use AXIAL coordinates for lookup
        Vector2Int key = new Vector2Int(coordinates.X, coordinates.Z);

        if (!cells.TryGetValue(key, out HexCell cell))
        {
            Debug.LogWarning($"PlaceObjectOnTile: Cell at coordinates {key.x},{key.y} does not exist.");
            return null;
        }

        if (cell.decorationObject != null)
        {
             if (Application.isPlaying)
                Destroy(cell.decorationObject);
            else
                DestroyImmediate(cell.decorationObject);
        }

        Vector3 spawnPos = cell.transform.position;
        cell.decorationObject = Instantiate(objectPrefab, spawnPos, Quaternion.identity, cell.transform);
        return cell.decorationObject;
    }

    public Vector3 GetWorldPositionFromCoordinates(int x, int z)
    {
        // This function now expects AXIAL coordinates (x, z)
        // Formula for axial to world position
        Vector3 position;
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        
        // The position calculated is local, convert to world position
        return transform.TransformPoint(position);
    }

    public HexCell[] GetAllCells()
    {
        return cells.Values.ToArray();
    }

    public HexCell GetCellAtCoordinates(int x, int z)
    {
        // Lookup using AXIAL coordinates
        cells.TryGetValue(new Vector2Int(x, z), out HexCell cell);
        return cell;
    }

    public HexCell GetOrCreateCellAt(int x, int z)
    {
        // This function now expects AXIAL coordinates (x, z)
        Vector2Int axialCoords = new Vector2Int(x, z);
        if (cells.TryGetValue(axialCoords, out HexCell cell) && cell != null)
        {
            return cell;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("cellPrefab is not assigned in HexGrid.");
            return null;
        }
        
        // Get world position from AXIAL coordinates
        Vector3 worldPosition = GetWorldPositionFromCoordinates(x, z);
        
        // Convert world position to local position for instantiating the cell
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        cell = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = localPosition;
        cell.coordinates = new HexCoordinates(x, z); // Assign axial coordinates

        cells[axialCoords] = cell;

        return cell;
    }

    public void ClearAllCells(bool isEditorMode = false)
    {
        if (cells == null)
        {
            return;
        }

        foreach (HexCell cell in cells.Values)
        {
            if (cell == null) continue;
            if (cell.currentTile != null)
            {
                if (isEditorMode || !Application.isPlaying) DestroyImmediate(cell.currentTile); else Destroy(cell.currentTile);
                cell.currentTile = null;
            }
            if (cell.decorationObject != null)
            {
                if (isEditorMode || !Application.isPlaying) DestroyImmediate(cell.decorationObject); else Destroy(cell.decorationObject);
                cell.decorationObject = null;
            }
        }
        Debug.Log("All cells have been cleared.");
#if UNITY_EDITOR
        if (isEditorMode) EditorUtility.SetDirty(this);
#endif
    }
}
