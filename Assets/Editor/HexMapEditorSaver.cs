using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.UI;

public class HexMapEditorSaver
{
    private static string lastUsedMapName = null;

    [MenuItem("Hex Editor/Save Chunked Map As...")]
    public static void SaveChunkedMap()
    {
        MapSaveLoadManager mapIOManager = GameObject.FindObjectOfType<MapSaveLoadManager>();
        if (mapIOManager == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy MapIOManager trong scene. Vui lòng thêm và cấu hình nó.", "OK");
            return;
        }

        // Đảm bảo MapIOManager có các tham chiếu cần thiết
        if (mapIOManager.grid == null) mapIOManager.grid = GameObject.FindObjectOfType<HexGrid>();

        // Mở dialog để người dùng nhập tên map (tên thư mục)
        string mapName = EditorInputDialog.Show("Save Map", "Nhập tên map (sẽ là tên thư mục):", lastUsedMapName ?? "MyNewMap");

        if (!string.IsNullOrEmpty(mapName))
        {
            SaveMapWithName(mapName, mapIOManager);
        }
        else
        {
            Debug.Log("Hủy lưu map.");
        }
    }

    [MenuItem("Hex Editor/Quick Save Map _F5")]
    public static void QuickSaveMap()
    {
        if (string.IsNullOrEmpty(lastUsedMapName))
        {
            // Nếu chưa có tên map, gọi SaveChunkedMap để yêu cầu người dùng nhập tên
            SaveChunkedMap();
            return;
        }

        MapSaveLoadManager mapIOManager = GameObject.FindObjectOfType<MapSaveLoadManager>();
        if (mapIOManager == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy MapIOManager trong scene. Vui lòng thêm và cấu hình nó.", "OK");
            return;
        }

        // Đảm bảo MapIOManager có các tham chiếu cần thiết
        if (mapIOManager.grid == null) mapIOManager.grid = GameObject.FindObjectOfType<HexGrid>();


        SaveMapWithName(lastUsedMapName, mapIOManager);
    }

    private static void SaveMapWithName(string mapName, MapSaveLoadManager mapIOManager)
    {
        // Kiểm tra ký tự không hợp lệ cho tên thư mục (đơn giản hóa)
        if (mapName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || mapName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            EditorUtility.DisplayDialog("Tên không hợp lệ", "Tên map chứa ký tự không hợp lệ cho tên thư mục/file.", "OK");
            return;
        }

        Debug.Log($"Đang lưu map: {mapName}...");
        mapIOManager.SaveMapByChunks(mapName); // Gọi hàm lưu của MapIOManager
        lastUsedMapName = mapName; // Lưu tên map để dùng cho lần sau
        string savePath = Path.Combine(Application.streamingAssetsPath, "Maps");
        EditorUtility.DisplayDialog("Lưu Thành Công", $"Map '{mapName}' đã được lưu vào {savePath}", "OK");
    }
}