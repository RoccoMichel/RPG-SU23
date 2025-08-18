using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TMP_Text playerStats;
    private List<ItemSlot> inventorySlots = new();
    private PlayerInventory playerInventory;
    private GameDirector director;

    private void Start()
    {
        playerInventory = director.player.inventory;

        foreach (ItemSlot slot in gameObject.GetComponentsInChildren<ItemSlot>())
            inventorySlots.Add(slot);

        RefreshInventory();
    }

    private void OnEnable()
    {
        if (director == null) director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();

        if (playerStats != null) ReloadStatistics();
        if (playerInventory != null) RefreshInventory();
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (i >= inventorySlots.Count) break;

            inventorySlots[i].Set(playerInventory.Items[i], true);
        }
    }

    public void RefreshInventory(int[] indexes)
    {
        foreach(int i in indexes)
        {
            inventorySlots[i].Set(playerInventory.Items[i], true);
        }
    }

    public void ReloadStatistics()
    {
        if (playerStats == null || director == null) return;

        playerStats.text = $@"Player Statistics:
MAX-HEALTH: {director.statistics.MaxHealth}
STRENGTH: {director.statistics.Strength}
DEFENSE: {director.statistics.Defense}
SPEED: {director.statistics.Speed}";
    }
}
