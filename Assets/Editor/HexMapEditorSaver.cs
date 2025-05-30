using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.UI;

public class HexMapEditorSaver
{
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
        if (mapIOManager.editor == null) mapIOManager.editor = GameObject.FindObjectOfType<HexMapEditor>(); // Hoặc HexMapEditor

        if (mapIOManager.grid == null || mapIOManager.editor == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "MapIOManager thiếu tham chiếu Grid hoặc Editor. Vui lòng cấu hình.", "OK");
            return;
        }

        // Mở dialog để người dùng nhập tên map (tên thư mục)
        string mapName = EditorInputDialog.Show("Save Map", "Nhập tên map (sẽ là tên thư mục):", "MyNewMap");

        if (!string.IsNullOrEmpty(mapName))
        {
            // Kiểm tra ký tự không hợp lệ cho tên thư mục (đơn giản hóa)
            if (mapName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || mapName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                EditorUtility.DisplayDialog("Tên không hợp lệ", "Tên map chứa ký tự không hợp lệ cho tên thư mục/file.", "OK");
                return;
            }

            Debug.Log($"Đang lưu map: {mapName}...");
            mapIOManager.SaveMapByChunks(mapName); // Gọi hàm lưu của MapIOManager
            EditorUtility.DisplayDialog("Lưu Thành Công", $"Map '{mapName}' đã được lưu vào {Application.persistentDataPath}", "OK");
        }
        else
        {
            Debug.Log("Hủy lưu map.");
        }
    }
}