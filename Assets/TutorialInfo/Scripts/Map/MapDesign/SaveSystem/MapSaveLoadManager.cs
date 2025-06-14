using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

public class MapSaveLoadManager : MonoBehaviour
{
    public HexGrid grid;
  
    public int chunkSize = 16;
    public MapObjectDatabase mapDatabase;

    private Dictionary<Vector2Int, Transform> chunkParentTransforms = new Dictionary<Vector2Int, Transform>(); // Để tổ chức scene
    public void SaveMapByChunks(string mapName)
    {
        if (grid == null)
        {
            Debug.LogError("HexGrid chưa có trong sence");
            return;
        }

        // Tạo thư mục cho map này nếu chưa tồn tại
#if UNITY_EDITOR
        string rootPath = Path.Combine(Application.streamingAssetsPath, "Maps");
        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }
        string mapDirectory = Path.Combine(rootPath, mapName);
#else
        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
#endif
        Directory.CreateDirectory(mapDirectory);

        // Phân nhóm các ô theo chunk ID
        Dictionary<Vector2Int, List<HexCell>> cellsByChunk = new Dictionary<Vector2Int, List<HexCell>>();

        foreach (HexCell cell in grid.GetAllCells()) // Giả sử grid.GetAllCells() trả về tất cả các ô
        {
            if (cell == null) continue;

            int chunkX = Mathf.FloorToInt((float)cell.coordinates.X / chunkSize);
            int chunkZ = Mathf.FloorToInt((float)cell.coordinates.Z / chunkSize);
            Vector2Int chunkId = new Vector2Int(chunkX, chunkZ);

            if (!cellsByChunk.ContainsKey(chunkId))
            {
                cellsByChunk[chunkId] = new List<HexCell>();
            }
            cellsByChunk[chunkId].Add(cell);
        }

        MasterMapData masterMapData = new MasterMapData
        {
            mapName = mapName,
            chunkSize = this.chunkSize,
            chunkFileNames = new List<string>()
        };

        if (cellsByChunk.Keys.Count > 0)
        {
            masterMapData.mapWidthInChunks = cellsByChunk.Keys.Max(id => id.x) + 1;
            masterMapData.mapHeightInChunks = cellsByChunk.Keys.Max(id => id.y) + 1;
        }
        else
        {
            masterMapData.mapWidthInChunks = 0;
            masterMapData.mapHeightInChunks = 0;
        }


        // Lưu từng chunk
        foreach (KeyValuePair<Vector2Int, List<HexCell>> chunkEntry in cellsByChunk)
        {
            Vector2Int chunkId = chunkEntry.Key;
            List<HexCell> cellsInChunk = chunkEntry.Value;

            HexChunkData chunkData = new HexChunkData
            {
                chunkX = chunkId.x,
                chunkZ = chunkId.y,
                tiles = new List<HexTileData>()
            };

            foreach (HexCell cell in cellsInChunk)
            {
                HexTileData tileData;
                if (cell.currentTile != null)
                {
                    tileData = new HexTileData
                    {
                        x = cell.coordinates.X, 
                        z = cell.coordinates.Z,
                        tilePrefabIndex = mapDatabase.GetTilePrefabIndex(cell.currentTile),
                        objectPrefabIndex = mapDatabase.GetObjectPrefabIndex(cell.decorationObject)
                    };
                }
                else
                {
                    tileData = new HexTileData
                    {
                        x = cell.coordinates.X,
                        z = cell.coordinates.Z,
                        tilePrefabIndex =-1,
                        objectPrefabIndex =-1
                    };
                }

                if (cell.currentTile != null)
                {
                    Vector3 tileRot = cell.currentTile.transform.eulerAngles;
                    tileData.tileRotX = tileRot.x;
                    tileData.tileRotY = tileRot.y;
                    tileData.tileRotZ = tileRot.z;
                }

                if (cell.decorationObject != null)
                {
                    Vector3 objRot = cell.decorationObject.transform.eulerAngles;
                    tileData.objRotX = objRot.x;
                    tileData.objRotY = objRot.y;
                    tileData.objRotZ = objRot.z;
                }
                chunkData.tiles.Add(tileData);
            }

            string chunkFileName = $"chunk_{chunkId.x}_{chunkId.y}.json";
            string chunkFilePath = Path.Combine(mapDirectory, chunkFileName);
            string json = JsonUtility.ToJson(chunkData, true);
            File.WriteAllText(chunkFilePath, json);
            masterMapData.chunkFileNames.Add(chunkFileName);
        }

        string masterFilePath = Path.Combine(mapDirectory, "master_map_data.json");
        string masterJson = JsonUtility.ToJson(masterMapData, true);
        File.WriteAllText(masterFilePath, masterJson);
        Debug.Log($"Map '{mapName}' đã được lưu với {cellsByChunk.Count} chunks vào thư mục: {mapDirectory}");
    }

    public MasterMapData LoadMasterMapData(string mapName)
    {
        string masterFilePath = Path.Combine(Application.persistentDataPath, mapName, "master_map_data.json");
        string json = null;

        if (File.Exists(masterFilePath))
        {
            json = File.ReadAllText(masterFilePath);
        }
        else
        {
            // Fallback for editor-only sync read from StreamingAssets
            masterFilePath = Path.Combine(Application.streamingAssetsPath, "Maps", mapName, "master_map_data.json");
             if (File.Exists(masterFilePath))
            {
                json = File.ReadAllText(masterFilePath);
            }
        }
    
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"Không tìm thấy file master map data cho '{mapName}' ở cả persistentDataPath và StreamingAssets/Maps.");
            return null;
        }

        MasterMapData masterData = JsonUtility.FromJson<MasterMapData>(json);
        if (masterData != null)
        {
            this.chunkSize = masterData.chunkSize;
        }
        return masterData;
    }

    public IEnumerator LoadMasterMapDataAsync(string mapName, Action<MasterMapData> onCompleted)
    {
        string masterFilePath = Path.Combine(Application.persistentDataPath, mapName, "master_map_data.json");
        string json = null;

        if (File.Exists(masterFilePath))
        {
            json = File.ReadAllText(masterFilePath);
        }
        else
        {
            // Fallback to StreamingAssets
            masterFilePath = Path.Combine(Application.streamingAssetsPath, "Maps", mapName, "master_map_data.json");
            yield return StartCoroutine(ReadAllTextAsync(masterFilePath, result => json = result));
        }
    
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"Không tìm thấy file master map data cho '{mapName}' ở cả persistentDataPath và StreamingAssets/Maps.");
            onCompleted?.Invoke(null);
            yield break;
        }

        MasterMapData masterData = JsonUtility.FromJson<MasterMapData>(json);
        if (masterData != null)
        {
            this.chunkSize = masterData.chunkSize;
        }
        onCompleted?.Invoke(masterData);
    }

    /// <summary>
    /// Tải tất cả các chunk của một map (thường dùng cho editor).
    /// </summary>
    public void LoadAllMapChunksForEditor(string mapName)
    {
        MasterMapData masterData = LoadMasterMapData(mapName);
        if (masterData == null) return;

        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
        // Check both paths for editor loading
        if (!Directory.Exists(mapDirectory))
        {
            mapDirectory = Path.Combine(Application.streamingAssetsPath, "Maps", mapName);
        }

        ClearAndResetMap(true); // isEditorMode = true

        Debug.Log($"Đang tải map '{mapName}' cho editor...");
        foreach (string chunkFileName in masterData.chunkFileNames)
        {
            string chunkFilePath = Path.Combine(mapDirectory, chunkFileName);
            LoadChunkFromFile(chunkFilePath, mapName, true); // isEditorMode = true
        }
        Debug.Log($"Map '{mapName}' đã được tải hoàn tất cho editor.");

#if UNITY_EDITOR
        if (grid != null) EditorUtility.SetDirty(grid);
        EditorUtility.SetDirty(this); // Đánh dấu MapIOManager là dirty để lưu các thay đổi (nếu có) vào scene
#endif
    }

    public IEnumerator LoadChunkForPlayer(string mapName, int chunkX, int chunkZ)
    {
        if (chunkParentTransforms.ContainsKey(new Vector2Int(chunkX, chunkZ)))
        {
            yield break; // Already loaded
        }

        string chunkFileName = $"chunk_{chunkX}_{chunkZ}.json";
        string chunkFilePath = Path.Combine(Application.persistentDataPath, mapName, chunkFileName);
        string json = null;

        if (File.Exists(chunkFilePath))
        {
            json = File.ReadAllText(chunkFilePath);
        }
        else
        {
            chunkFilePath = Path.Combine(Application.streamingAssetsPath, "Maps", mapName, chunkFileName);
            yield return StartCoroutine(ReadAllTextAsync(chunkFilePath, result => json = result));
        }

        if (!string.IsNullOrEmpty(json))
        {
            LoadChunkFromJson(json, mapName, false);
        }
    }

    private void LoadChunkFromFile(string chunkFilePath, string mapName, bool isEditorMode = false)
    {
        if (!File.Exists(chunkFilePath))
        {
            Debug.LogWarning($"Không tìm thấy file chunk: {chunkFilePath}");
            return;
        }
        string json = File.ReadAllText(chunkFilePath);
        LoadChunkFromJson(json, mapName, isEditorMode);
    }

    private void LoadChunkFromJson(string json, string mapName, bool isEditorMode)
    {
        HexChunkData chunkData = JsonUtility.FromJson<HexChunkData>(json);

        if (chunkData == null)
        {
            Debug.LogError($"Không thể phân tích dữ liệu chunk từ JSON.");
            return;
        }

        Transform chunkParent = GetOrCreateChunkParent(chunkData.chunkX, chunkData.chunkZ, mapName);

        foreach (HexTileData tileData in chunkData.tiles)
        {
            HexCell cell = grid.GetOrCreateCellAt(tileData.x, tileData.z);

            if (cell == null)
            {
                Debug.LogWarning($"Ô tại {tileData.x},{tileData.z} không tìm thấy hoặc không thể tạo. Bỏ qua tile này.");
                continue;
            }

            Vector3 position = grid.GetWorldPositionFromCoordinates(tileData.x, tileData.z);

            // Xóa tile và object cũ (nếu có)
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

            // Tải tile (địa hình)
            if (tileData.tilePrefabIndex >= 0 && tileData.tilePrefabIndex < mapDatabase.tileCount)
            {
                GameObject tilePrefab = mapDatabase.GetTile(tileData.tilePrefabIndex);
                GameObject tileObj;
                if (isEditorMode)
                {
#if UNITY_EDITOR
                    tileObj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, chunkParent);
#else
                    tileObj = Instantiate(tilePrefab, chunkParent); 
#endif
                }
                else
                {
                    tileObj = Instantiate(tilePrefab, chunkParent);
                }
                tileObj.transform.position = position;
                tileObj.transform.eulerAngles = new Vector3(tileData.tileRotX, tileData.tileRotY, tileData.tileRotZ);
                cell.currentTile = tileObj;
            }
            else if (tileData.tilePrefabIndex != -1)
            {
                Debug.LogWarning($"TilePrefabIndex không hợp lệ {tileData.tilePrefabIndex} cho ô {tileData.x},{tileData.z}");
            }

            // Tải object trang trí
            if (tileData.objectPrefabIndex >= 0 && tileData.objectPrefabIndex < mapDatabase.objectCount)
            {
                GameObject objectPrefab = mapDatabase.GetObject(tileData.objectPrefabIndex);
                GameObject obj;
                Transform decorationParent = chunkParent;

                if (isEditorMode)
                {
#if UNITY_EDITOR
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(objectPrefab, decorationParent);
#else
                    obj = Instantiate(objectPrefab, decorationParent); // Fallback
#endif
                }
                else
                {
                    obj = Instantiate(objectPrefab, decorationParent);
                }
                // Giữ lại logic offset Y từ code gốc của HexMapEditorLoader
                obj.transform.position = position + new Vector3(0, -0.2f, 0);
                obj.transform.eulerAngles = new Vector3(tileData.objRotX, tileData.objRotY, tileData.objRotZ);
                cell.decorationObject = obj;
            }
            else if (tileData.objectPrefabIndex != -1)
            {
                Debug.LogWarning($"ObjectPrefabIndex không hợp lệ {tileData.objectPrefabIndex} cho ô {tileData.x},{tileData.z}");
            }
        }
    }

    public void UnloadChunk(int chunkX, int chunkZ)
    {
        Vector2Int chunkId = new Vector2Int(chunkX, chunkZ);
        if (chunkParentTransforms.TryGetValue(chunkId, out Transform chunkParent))
        {
            if (chunkParent != null) // Kiểm tra xem transform có còn tồn tại không
            {
                // Hủy tất cả các GameObjects con của chunk parent
                // và cập nhật lại trạng thái của HexCell
                int startX = chunkX * chunkSize;
                int endX = startX + chunkSize;
                int startZ = chunkZ * chunkSize;
                int endZ = startZ + chunkSize;

                for (int x = startX; x < endX; x++)
                {
                    for (int z = startZ; z < endZ; z++)
                    {
                        HexCell cell = grid.GetCellAtCoordinates(x,z);
                        if (cell != null)
                        {
                            // Quan trọng: Chỉ xóa nếu object đó là con của chunk đang unload
                            if (cell.currentTile != null && cell.currentTile.transform.parent == chunkParent)
                            {
                                // Destroy(cell.currentTile); // Sẽ được hủy cùng chunkParent
                                cell.currentTile = null;
                            }
                            if (cell.decorationObject != null && cell.decorationObject.transform.parent == chunkParent)
                            {
                                // Destroy(cell.decorationObject); // Sẽ được hủy cùng chunkParent
                                cell.decorationObject = null;
                            }
                        }
                    }
                }
                Destroy(chunkParent.gameObject); // Hủy GameObject cha của chunk
            }
            chunkParentTransforms.Remove(chunkId); // Xóa khỏi dictionary
        }
    }

    /// <summary>
    /// Lấy hoặc tạo GameObject cha cho một chunk để tổ chức Scene Hierarchy.
    /// </summary>
    private Transform GetOrCreateChunkParent(int chunkX, int chunkZ, string mapName)
    {
        Vector2Int chunkId = new Vector2Int(chunkX, chunkZ);
        if (!chunkParentTransforms.TryGetValue(chunkId, out Transform parent) || parent == null)
        {
            // Tìm hoặc tạo GameObject cho map tổng thể
            Transform mapRoot = transform.Find(mapName); // Tìm con có tên là mapName
            if (mapRoot == null)
            {
                GameObject mapRootGO = new GameObject(mapName);
                mapRoot = mapRootGO.transform;
                mapRoot.SetParent(this.transform); // Đặt nó làm con của MapIOManager cho gọn
            }


            GameObject chunkGO = new GameObject($"Chunk_{chunkX}_{chunkZ}");
            chunkGO.transform.SetParent(mapRoot); // Đặt làm con của mapRoot
            parent = chunkGO.transform;
            chunkParentTransforms[chunkId] = parent;
        }
        return parent;
    }

    /// <summary>
    /// Xóa toàn bộ map hiện tại và reset trạng thái quản lý chunk.
    /// Thường dùng trước khi tải một map mới hoàn toàn trong editor.
    /// </summary>
    public void ClearAndResetMap(bool isEditorMode = false)
    {
        foreach (var pair in chunkParentTransforms)
        {
            if (pair.Value != null)
            {
                if (isEditorMode) DestroyImmediate(pair.Value.gameObject); else Destroy(pair.Value.gameObject);
            }
        }
        chunkParentTransforms.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            // Kiểm tra xem child có phải là map root không (ví dụ, có tên là mapName)
            // Hoặc đơn giản là xóa tất cả các con nếu MapIOManager không có con nào khác ngoài map roots.
            if (isEditorMode) DestroyImmediate(child.gameObject); else Destroy(child.gameObject);
        }

        if (grid != null)
        {
            grid.ClearAllCells(isEditorMode);
        }
        Debug.Log("Map đã được dọn dẹp và trạng thái quản lý chunk đã được reset.");
    }

    private IEnumerator ReadAllTextAsync(string filePath, Action<string> onCompleted)
    {
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    // Don't log error if file not found, it's an expected case
                    // Debug.LogWarning($"Failed to load file at {filePath}: {www.error}");
                    onCompleted?.Invoke(null);
                }
                else
                {
                    onCompleted?.Invoke(www.downloadHandler.text);
                }
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                onCompleted?.Invoke(File.ReadAllText(filePath));
            }
            else
            {
                onCompleted?.Invoke(null);
            }
        }
    }
}