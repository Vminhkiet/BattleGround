using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class MasterMapData
{
    public string mapName;
    public int chunkSize; 
    public List<string> chunkFileNames = new(); 
    public int mapWidthInChunks;
    public int mapHeightInChunks;
}