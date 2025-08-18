using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemSlot> Items = new();
    private GameDirector Director { get => GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>(); set => Director = value; }

    [System.Serializable]
    public struct ItemSlot
    {
        public Item item;
        public int amount;
    }
    
    public void AddAmountAtIndex(int index, int amount)
    {
        var slot = Items[index];
        slot.amount += amount;
        Items[index] = slot;
    }

    public int[] GetSlotIndexesByName(string name)
    {
        List<int> results = new();

        for (int i = 0; i < Items.Count; i++)
        {
            ItemSlot slot = Items[i];

            if (slot.item.itemName == name && slot.amount < slot.item.stackSize)
                results.Add(i);
        }

        return results.ToArray();
    }

    public void AddItem(Item item, int amount)
    {
        int[] possibleIndexes = GetSlotIndexesByName(item.itemName);
        List<int> changes = new();

        // Increase amount on existing itemSlot(s)
        if (possibleIndexes != null)
        {
            foreach (int i in possibleIndexes)
            {
                int addableAmount = Items[i].item.stackSize - Items[i].amount;

                if (addableAmount >= amount)
                {
                    changes.Add(i);
                    AddAmountAtIndex(i, amount);
                    amount = 0;
                    break;
                }
                else
                {
                    changes.Add(i);
                    amount -= addableAmount;
                    AddAmountAtIndex(i, addableAmount);
                }

            }
        }

        // add amount to new itemSlot(s)
        while (amount > 0)
        {
            // Items.Count is correct because target element get added next line in the back of the list
            int i = Items.Count;
            Items.Add(new ItemSlot { item = item, amount = 0 });

            int addableAmount = Items[i].item.stackSize - Items[i].amount;

            if (addableAmount >= amount)
            {
                changes.Add(i);
                AddAmountAtIndex(i, amount);
                amount = 0;
            }
            else
            {
                changes.Add(i);
                amount -= addableAmount;
                AddAmountAtIndex(i, addableAmount);
            }
        }

        // Refresh visual changes on effected slots
        try { Director.canvasManager.inventory.RefreshInventory(changes.ToArray()); }
        catch{ }
    }

    //public void RemoveItem(Item item, int amount)
    //{

    //}

    //public void RemoveItem(Item item, int amount)
    //{
    //    Items.RemoveAt(0);
    //}
}
