using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

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
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        // Instantiate the default tile prefab and parent it to the cell
        if (defaultTilePrefab != null)
        {
            GameObject tile = Instantiate(defaultTilePrefab, cell.transform.position, Quaternion.identity, cell.transform);
            cell.currentTile = tile;
        }

        // Create coordinate label (optional but cute)
      /*  Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();*/
    }

    public void ReplaceCellPrefab(Vector3 position, GameObject newPrefab)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

        if (index < 0 || index >= cells.Length) return;

        HexCell cell = cells[index];

        // Remove current tile GameObject
        if (cell.currentTile != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(cell.currentTile);
#else
            Destroy(cell.currentTile);
#endif
        }

        // Instantiate and assign new prefab
        Vector3 worldPos = cell.transform.position;
        GameObject newTile = Instantiate(newPrefab, worldPos, Quaternion.identity, cell.transform);
        cell.currentTile = newTile;
    }
}
