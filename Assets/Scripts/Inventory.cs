using TMPro;
using UnityEngine;
using System.Collections.Generic;

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

        for (int i = 0; i < playerInventory.Items.Count; i++)
        {
            if (i >= inventorySlots.Count) break;

            inventorySlots[i].Set(playerInventory.Items[i], true);
        }
    }

    private void OnEnable()
    {
        if (director == null) director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();

        if (playerStats != null) ReloadStatistics();
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
