using UnityEngine;
using UnityEditor;
using System.IO;

public class HexMapEditorLoader
{
    [MenuItem("Hex Editor/Load Map From JSON")]
    public static void LoadMapFromJson()
    {
        string path = EditorUtility.OpenFilePanel("Select Map JSON", "Assets/Maps", "json");
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Load cancelled.");
            return;
        }

        HexGrid grid = GameObject.FindObjectOfType<HexGrid>();
        HexMapEditor editor = GameObject.FindObjectOfType<HexMapEditor>();

        if (grid == null || editor == null)
        {
            Debug.LogError("HexGrid or HexMapEditor not found in the scene!");
            return;
        }

        string json = File.ReadAllText(path);
        HexMapSaveData saveData = JsonUtility.FromJson<HexMapSaveData>(json);

        foreach (HexTileData tile in saveData.tiles)
        {
            Vector3 position = grid.GetWorldPositionFromCoordinates(tile.x, tile.z);

            // Instantiate tile prefab
            GameObject tilePrefab = editor.tilePrefabs[tile.tilePrefabIndex];
            GameObject tileObj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, grid.transform);
            tileObj.transform.position = position;
            tileObj.transform.rotation = Quaternion.Euler(tile.tileRotX, tile.tileRotY, tile.tileRotZ);

            // Attach to a HexCell manually if needed
            HexCell cell = grid.GetCellAtCoordinates(tile.x, tile.z);
            if (cell != null)
            {
                cell.currentTile = tileObj;
            }

            // Instantiate decoration object
            if (tile.objectPrefabIndex >= 0)
            {
                GameObject objectPrefab = editor.objectPrefabs[tile.objectPrefabIndex];
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(objectPrefab, tileObj.transform);
                obj.transform.position = position + new Vector3(0, 0.5f, 0);
                obj.transform.rotation = Quaternion.Euler(tile.objRotX, tile.objRotY, tile.objRotZ);

                if (cell != null)
                {
                    cell.decorationObject = obj;
                }
            }
        }

        Debug.Log("Map loaded into scene in Edit Mode.");
    }
}