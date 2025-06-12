
using UnityEngine;

[CreateAssetMenu(fileName = "MapObjectDatabase", menuName = "Map/Map object database")]
public class MapObjectDatabase : ScriptableObject
{
    public GameObject[] tilePrefabs;
    public GameObject[] objectPrefabs;

    public int tileCount
    {
        get { return tilePrefabs.Length; }
    }

    public int objectCount
    {
        get { return objectPrefabs.Length; }
    }
    public GameObject GetTile(int index)
    {
        return tilePrefabs[index];
    }

    public GameObject GetObject(int index)
    {
        return objectPrefabs[index];
    }

    public int GetTilePrefabIndex(GameObject instance)
    {
        for (int i = 0; i < tilePrefabs.Length; i++)
            if (instance.name.Contains(tilePrefabs[i].name)) return i;

        return -1;
    }

    public int GetObjectPrefabIndex(GameObject instance)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
            if (instance != null && instance.name.Contains(objectPrefabs[i].name)) return i;

        return -1;
    }
}
