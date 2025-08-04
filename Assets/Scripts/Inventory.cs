using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TMP_Text playerStats;
    private PlayerInventory playerInventory;
    private GameDirector director;

    private void OnEnable()
    {
        if (director == null) director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        if (playerStats == null) return;
        ReloadStatistics();
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
