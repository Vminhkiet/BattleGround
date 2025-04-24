using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Map
{
    public Sprite sprite;
    public string name;
    [Range(0,3)]
    public int id_map;
}
