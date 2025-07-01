using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadMapHandler : MonoBehaviour
{
    private MapSaveLoadManager mapSaveLoadManager;
    public string mapName = "DefaultMap"; // Tên map bạn muốn tải

    [Header("Chunk Loading Settings")]
    public int loadRadius = 2; // Bán kính (tính bằng chunk) xung quanh người chơi sẽ được tải
    public float checkInterval = 1.0f; // Thời gian (giây) giữa mỗi lần kiểm tra để tải/gỡ chunk

    private Vector2Int currentPlayerChunk;
    private HashSet<Vector2Int> loadedChunks = new HashSet<Vector2Int>();
    private MasterMapData masterMapData;
    private bool isProcessing = false;

    IEnumerator Start()
    {
        mapSaveLoadManager = FindAnyObjectByType<MapSaveLoadManager>();
        if (mapSaveLoadManager == null)
        {
            Debug.LogError("MapSaveLoadManager chưa được gán!");
            yield break;
        }

        yield return StartCoroutine(mapSaveLoadManager.LoadMasterMapDataAsync(mapName, (data) => {
            masterMapData = data;
        }));

        if (masterMapData == null)
        {
            Debug.LogError($"Không thể tải master data cho map '{mapName}'.");
            yield break;
        }

        // Bắt đầu coroutine để kiểm tra và cập nhật chunk
        StartCoroutine(CheckAndLoadChunksCoroutine());
    }

    private Vector2Int GetChunkIdFromPosition(Vector3 position)
    {
        // Chuyển đổi world position sang HexCoordinates
        HexCoordinates coords = HexCoordinates.FromPosition(mapSaveLoadManager.grid.transform.InverseTransformPoint(position));
        
        // Tính toán chunk ID từ tọa độ ô
        int chunkX = Mathf.FloorToInt((float)coords.X / mapSaveLoadManager.chunkSize);
        int chunkZ = Mathf.FloorToInt((float)coords.Z / mapSaveLoadManager.chunkSize);

        return new Vector2Int(chunkX, chunkZ);
    }

    private IEnumerator CheckAndLoadChunksCoroutine()
    {
        while (true)
        {
            if (!isProcessing)
            {
                Vector2Int newPlayerChunk = GetChunkIdFromPosition(transform.position);
                if (newPlayerChunk != currentPlayerChunk)
                {
                    currentPlayerChunk = newPlayerChunk;
                    // Bắt đầu quá trình xử lý chunk trong một coroutine riêng để không chặn vòng lặp chính
                    StartCoroutine(UpdateVisibleChunks());
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator UpdateVisibleChunks()
    {
        isProcessing = true;
        HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();

        // Xác định các chunk cần thiết dựa trên bán kính
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int z = -loadRadius; z <= loadRadius; z++)
            {
                requiredChunks.Add(new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + z));
            }
        }

        // Gỡ bỏ các chunk không cần thiết
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();
        foreach (var chunkId in loadedChunks)
        {
            if (!requiredChunks.Contains(chunkId))
            {
                chunksToUnload.Add(chunkId);
            }
        }

        foreach (var chunkId in chunksToUnload)
        {
            mapSaveLoadManager.UnloadChunk(chunkId.x, chunkId.y);
            loadedChunks.Remove(chunkId);
            // Debug.Log($"Đã gỡ chunk: {chunkId}");
            yield return null; // Chờ một frame để phân bổ tác vụ
        }
        
        // Tải các chunk mới
        foreach (var chunkId in requiredChunks)
        {
            if (!loadedChunks.Contains(chunkId))
            {
                // Tải chunk bằng coroutine
                StartCoroutine(mapSaveLoadManager.LoadChunkForPlayer(mapName, chunkId.x, chunkId.y));
                loadedChunks.Add(chunkId);
                // Debug.Log($"Đã yêu cầu tải chunk: {chunkId}");
                yield return null; // Chờ một frame để phân bổ tác vụ
            }
        }

        isProcessing = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
