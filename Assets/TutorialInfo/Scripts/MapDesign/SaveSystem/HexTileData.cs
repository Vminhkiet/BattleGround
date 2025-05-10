
[System.Serializable]
public class HexTileData
{
    public int x, z;
    public int tilePrefabIndex;
    public int objectPrefabIndex;
    // Tile rotation
    public float tileRotX, tileRotY, tileRotZ;

    // Object rotation
    public float objRotX, objRotY, objRotZ;
}