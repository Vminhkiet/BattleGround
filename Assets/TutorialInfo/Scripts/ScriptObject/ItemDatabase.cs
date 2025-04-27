using UnityEngine;

[CreateAssetMenu(fileName = "ItempDatabase", menuName = "Shopping/ Item shop database")]
public class ItemDatabase : ScriptableObject
{
    public Item[] items;

    public int ItemCount
    {
        get { return items.Length; }
    }

    public Item GetItem(int index)
    {
        return items[index];
    }

    public void PurchaseItem(int index)
    {
        items[index].isPurchased = true;
    }
}

