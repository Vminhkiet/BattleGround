using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct Item
{
    public Sprite image;
    public string name;
    public int price;
    public bool isPurchased;
}
