using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using UnityEngine;

public class MapSaveLoadManager : MonoBehaviour
{
    public HexGrid grid;
    public HexMapEditor editor; // needed to get prefab lists

    public void SaveMap(string filePath)
    {
        HexMapSaveData saveData = new();

        foreach (HexCell cell in grid.GetAllCells())
        {
            HexTileData tile = new()
            {
                x = cell.coordinates.X,
                z = cell.coordinates.Z,
                tilePrefabIndex = editor.GetTilePrefabIndex(cell.currentTile),
                objectPrefabIndex = editor.GetObjectPrefabIndex(cell.decorationObject)
            };

            if (cell.currentTile != null)
            {
                Vector3 tileRot = cell.currentTile.transform.eulerAngles;
                tile.tileRotX = tileRot.x;
                tile.tileRotY = tileRot.y;
                tile.tileRotZ = tileRot.z;
            }

            if (cell.decorationObject != null)
            {
                Vector3 objRot = cell.decorationObject.transform.eulerAngles;
                tile.objRotX = objRot.x;
                tile.objRotY = objRot.y;
                tile.objRotZ = objRot.z;
            }

            saveData.tiles.Add(tile);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Map saved to: " + filePath);
    }

    public void LoadMap(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        HexMapSaveData saveData = JsonUtility.FromJson<HexMapSaveData>(json);

        foreach (HexTileData tile in saveData.tiles)
        {
            Vector3 position = grid.GetWorldPositionFromCoordinates(tile.x, tile.z);

            // Load tile
            GameObject tileObj = grid.ReplaceCellPrefab(position, editor.tilePrefabs[tile.tilePrefabIndex]);
            if (tileObj != null)
            {
                tileObj.transform.eulerAngles = new Vector3(tile.tileRotX, tile.tileRotY, tile.tileRotZ);
            }

            // Load decoration object
            if (tile.objectPrefabIndex >= 0)
            {
                GameObject obj = grid.PlaceObjectOnTile(position, editor.objectPrefabs[tile.objectPrefabIndex]);
                if (obj != null)
                {
                    obj.transform.eulerAngles = new Vector3(tile.objRotX, tile.objRotY, tile.objRotZ);
                }
            }
        }

        Debug.Log("Map loaded from: " + filePath);
    }

}
