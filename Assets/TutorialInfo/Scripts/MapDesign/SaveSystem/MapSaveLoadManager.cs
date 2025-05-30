using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using UnityEngine;
using UnityEditor;

public class MapSaveLoadManager : MonoBehaviour
{
    public HexGrid grid;
    public HexMapEditor editor; // needed to get prefab lists
    public int chunkSize = 16; // Kích thước chunk (ví dụ: 16x16 ô), có thể tùy chỉnh

    private Dictionary<Vector2Int, Transform> chunkParentTransforms = new Dictionary<Vector2Int, Transform>(); // Để tổ chức scene
    public void SaveMapByChunks(string mapName)
    {
        if (grid == null || editor == null)
        {
            Debug.LogError("HexGrid hoặc MapEditor chưa được gán cho MapIOManager!");
            return;
        }

        // Tạo thư mục cho map này nếu chưa tồn tại
        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
        Directory.CreateDirectory(mapDirectory);

        // Phân nhóm các ô theo chunk ID
        Dictionary<Vector2Int, List<HexCell>> cellsByChunk = new Dictionary<Vector2Int, List<HexCell>>();

        foreach (HexCell cell in grid.GetAllCells()) // Giả sử grid.GetAllCells() trả về tất cả các ô
        {
            if (cell == null) continue;

            // Tính toán ID của chunk dựa trên tọa độ ô
            // Quan trọng: Đảm bảo cell.coordinates.X và Z là tọa độ toàn cục của ô
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
                HexTileData tileData = new HexTileData
                {
                    x = cell.coordinates.X, // Lưu tọa độ toàn cục của ô
                    z = cell.coordinates.Z,
                    tilePrefabIndex = editor.GetTilePrefabIndex(cell.currentTile),
                    objectPrefabIndex = editor.GetObjectPrefabIndex(cell.decorationObject) // Sẽ là -1 nếu không có object
                };

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
            // Debug.Log($"Chunk đã lưu: {chunkFilePath}");
            masterMapData.chunkFileNames.Add(chunkFileName);
        }

        // Lưu file master data của map
        string masterFilePath = Path.Combine(mapDirectory, "master_map_data.json");
        string masterJson = JsonUtility.ToJson(masterMapData, true);
        File.WriteAllText(masterFilePath, masterJson);
        Debug.Log($"Map '{mapName}' đã được lưu với {cellsByChunk.Count} chunks vào thư mục: {mapDirectory}");
    }

    /// <summary>
    /// Tải file master data của map.
    /// </summary>
    public MasterMapData LoadMasterMapData(string mapName)
    {
        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
        string masterFilePath = Path.Combine(mapDirectory, "master_map_data.json");

        if (!File.Exists(masterFilePath))
        {
            Debug.LogWarning($"Không tìm thấy file master map data: {masterFilePath}");
            return null;
        }

        string masterJson = File.ReadAllText(masterFilePath);
        MasterMapData masterData = JsonUtility.FromJson<MasterMapData>(masterJson);
        if (masterData != null)
        {
            this.chunkSize = masterData.chunkSize; // Cập nhật chunkSize từ file đã lưu
        }
        return masterData;
    }

    /// <summary>
    /// Tải tất cả các chunk của một map (thường dùng cho editor).
    /// </summary>
    public void LoadAllMapChunksForEditor(string mapName)
    {
        MasterMapData masterData = LoadMasterMapData(mapName);
        if (masterData == null) return;

        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
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
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = grid.transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }



    /// <summary>
    /// Tải một chunk cụ thể dựa vào ID của nó (dùng cho người chơi).
    /// </summary>
    public void LoadChunkForPlayer(string mapName, int chunkX, int chunkZ)
    {
        // Kiểm tra xem chunk đã được tải chưa thông qua chunkParentTransforms
        if (chunkParentTransforms.ContainsKey(new Vector2Int(chunkX, chunkZ)))
        {
            // Debug.Log($"Chunk {chunkX},{chunkZ} của map '{mapName}' đã được tải trước đó.");
            return;
        }


        string mapDirectory = Path.Combine(Application.persistentDataPath, mapName);
        string chunkFileName = $"chunk_{chunkX}_{chunkZ}.json";
        string chunkFilePath = Path.Combine(mapDirectory, chunkFileName);

        if (File.Exists(chunkFilePath))
        {
            Debug.Log($"Đang tải chunk {chunkX},{chunkZ} cho map '{mapName}'...");
            LoadChunkFromFile(chunkFilePath, mapName);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy file chunk: {chunkFilePath}. Chunk này có thể không tồn tại.");
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
        HexChunkData chunkData = JsonUtility.FromJson<HexChunkData>(json);

        if (chunkData == null)
        {
            Debug.LogError($"Không thể phân tích dữ liệu chunk từ: {chunkFilePath}");
            return;
        }

        Transform chunkParent = GetOrCreateChunkParent(chunkData.chunkX, chunkData.chunkZ, mapName);

        foreach (HexTileData tileData in chunkData.tiles)
        {
            HexCell cell = grid.GetCellAtCoordinates(tileData.x, tileData.z);

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
            if (tileData.tilePrefabIndex >= 0 && tileData.tilePrefabIndex < editor.tilePrefabs.Length)
            {
                GameObject tilePrefab = editor.tilePrefabs[tileData.tilePrefabIndex];
                GameObject tileObj;
                if (isEditorMode)
                {
#if UNITY_EDITOR
                    tileObj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, chunkParent);
#else
                    tileObj = Instantiate(tilePrefab, chunkParent); // Fallback cho non-editor
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
            if (tileData.objectPrefabIndex >= 0 && tileData.objectPrefabIndex < editor.objectPrefabs.Length)
            {
                GameObject objectPrefab = editor.objectPrefabs[tileData.objectPrefabIndex];
                GameObject obj;
                // Vị trí gốc của object trang trí trong code cũ của bạn có offset Y.
                // Và được parent vào tileObj.transform. Hiện tại đang parent vào chunkParent.
                // Nếu muốn parent vào tileObj, thay chunkParent bằng cell.currentTile.transform (sau khi tileObj đã được tạo)
                // Và đảm bảo cell.currentTile không null.
                Transform decorationParent = chunkParent; // Hoặc cell.currentTile?.transform ?? chunkParent;

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
                obj.transform.position = position + new Vector3(0, 0.5f, 0);
                obj.transform.eulerAngles = new Vector3(tileData.objRotX, tileData.objRotY, tileData.objRotZ);
                cell.decorationObject = obj;
            }
            else if (tileData.objectPrefabIndex != -1)
            {
                Debug.LogWarning($"ObjectPrefabIndex không hợp lệ {tileData.objectPrefabIndex} cho ô {tileData.x},{tileData.z}");
            }
        }
    }


    /// <summary>
    /// Dọn dẹp một chunk cụ thể khỏi scene (dùng cho người chơi khi di chuyển xa).
    /// </summary>
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
            Debug.Log($"Chunk {chunkX},{chunkZ} đã được dọn dẹp.");
        }
        else
        {
            Debug.LogWarning($"Không thể dọn dẹp chunk {chunkX},{chunkZ}: không tìm thấy trong danh sách đã tải.");
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
}