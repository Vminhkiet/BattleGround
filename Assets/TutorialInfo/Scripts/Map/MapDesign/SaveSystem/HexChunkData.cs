using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class HexChunkData
{
    public int chunkX; // ID tọa độ X của chunk
    public int chunkZ; // ID tọa độ Z của chunk
    public List<HexTileData> tiles = new(); // Danh sách các ô trong chunk này
}