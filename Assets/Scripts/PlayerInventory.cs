using System;
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

    // ADDING ITEMS
    public void AddItem(Item item, int amount)
    {
        if (amount <= 0) return;

        Director.ReportItem(item);

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

    public void AddItem(Item item, int amount, bool notification)
    {
        if (amount <= 0) return;

        AddItem(item, amount);
        if (notification) Director.canvasManager.Notification($"+ {amount} {item.itemName}");
    }

    // REMOVING ITEMS
    public void RemoveItem(Item item, int amount)
    {
        if (amount <= 0) return;

        GetSlotIndexesByName(item.itemName);
        int[] slots = GetSlotIndexesByName(item.itemName);
        Array.Sort(slots);
        Array.Reverse(slots);

        for(int i= 0; i < slots.Length; i++)
        {
            int index = slots[i];

            if (Items[index].amount > amount)
            {
                AddAmountAtIndex(index, -amount);
                break;
            }
            else
            {
                amount -= Items[index].amount;
                Items.RemoveAt(index);
            }
        }
    }

    public void RemoveItem(Item item, int amount, bool notification)
    {
        if (amount <= 0) return;

        RemoveItem(item, amount);
        if (notification) Director.canvasManager.Notification($"- {amount} {item.itemName}");
    }

    public bool TryRemoveItem(Item item, int amount)
    {
        if (amount <= 0) return false;

        int availableAmount = 0;
        int[] slots = GetSlotIndexesByName(item.itemName);

        foreach (int i in slots)  availableAmount += Items[i].amount;

        // Not enough items to remove in player inventory
        if (availableAmount < amount) return false; 

        RemoveItem(item, amount);
        return true;
    }

    public bool TryRemoveItem(Item item, int amount, bool notification)
    {
        if (amount <= 0) return false;

        bool success = TryRemoveItem(item, amount);

        if (success && notification) Director.canvasManager.Notification($"- {amount} {item.itemName}");

        return success;
    }
}
