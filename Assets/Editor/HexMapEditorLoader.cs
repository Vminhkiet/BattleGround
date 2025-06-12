using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.UI;

public class HexMapEditorLoader
{
    [MenuItem("Hex Editor/Load Chunked Map To Scene")]
    public static void LoadChunkedMap()
    {
        // Mở dialog để người dùng chọn file "master_map_data.json"
        // Các map được lưu trong Application.streamingAssetsPath/Maps
        string mapsPath = Path.Combine(Application.streamingAssetsPath, "Maps");
        if (!Directory.Exists(mapsPath))
        {
            Directory.CreateDirectory(mapsPath);
        }
        string path = EditorUtility.OpenFilePanel("Select Master Map Data File (master_map_data.json)", mapsPath, "json");

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Hủy tải map.");
            return;
        }

        // Kiểm tra xem file được chọn có phải là "master_map_data.json" không
        if (!Path.GetFileName(path).Equals("master_map_data.json", System.StringComparison.OrdinalIgnoreCase))
        {
            EditorUtility.DisplayDialog("Sai File", "Vui lòng chọn file 'master_map_data.json'.", "OK");
            Debug.LogWarning("File được chọn không đúng. Vui lòng chọn file 'master_map_data.json'.");
            return;
        }

        // Trích xuất tên map từ cấu trúc thư mục
        // Đường dẫn sẽ có dạng: .../PersistentDataPath/TenMapCuaBan/master_map_data.json
        // Chúng ta cần "TenMapCuaBan"
        string mapDirectory = Path.GetDirectoryName(path);
        string mapName = Path.GetFileName(mapDirectory);

        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("Không thể xác định tên map từ đường dẫn file đã chọn.");
            return;
        }

        // Tìm instance của MapIOManager trong scene
        MapSaveLoadManager mapIOManager = GameObject.FindObjectOfType<MapSaveLoadManager>();
        if (mapIOManager == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy MapIOManager trong scene. Vui lòng thêm và cấu hình MapIOManager.", "OK");
            Debug.LogError("Không tìm thấy MapIOManager trong scene. Vui lòng thêm MapIOManager vào scene và gán các tham chiếu HexGrid, MapEditor.");
            return;
        }

        // Đảm bảo MapIOManager có các tham chiếu cần thiết
        if (mapIOManager.grid == null)
        {
            HexGrid gridInScene = GameObject.FindObjectOfType<HexGrid>();
            if (gridInScene != null)
            {
                mapIOManager.grid = gridInScene;
                EditorUtility.SetDirty(mapIOManager); // Lưu thay đổi vào MapIOManager
            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi", "MapIOManager thiếu tham chiếu HexGrid, và không tìm thấy HexGrid trong scene.", "OK");
                Debug.LogError("Build Grid trước!");
                return;
            }
        }

        EditorUtility.SetDirty(mapIOManager);

        mapIOManager.LoadAllMapChunksForEditor(mapName);
    }
    [MenuItem("Hex Editor/Clear Map")]
    public static void ClearLoadedMap()
    {
        MapSaveLoadManager mapIOManager = GameObject.FindObjectOfType<MapSaveLoadManager>();
        HexGrid gridInScene = GameObject.FindObjectOfType<HexGrid>();

        for (int i = mapIOManager.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = mapIOManager.transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }
        for (int i = gridInScene.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = gridInScene.transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }

    }
}