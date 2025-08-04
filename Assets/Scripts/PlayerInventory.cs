using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemSlot> Items = new();

    [System.Serializable]
    public struct ItemSlot
    {
        public Item item;
        public int amount;
    }

    public void AddItem(Item item, int amount)
    {
        Items.Add(new ItemSlot{ item = item, amount = amount });
    }
    public void RemoveItem(Item item, int amount)
    {
        Items.RemoveAt(0);
    }
}
