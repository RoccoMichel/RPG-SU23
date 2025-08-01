using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TMP_Text playerStats;
    private PlayerBase player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
        ReloadStatistics();
    }

    public void ReloadStatistics()
    {
        if (playerStats == null) return;

        playerStats.text = $@"Player Statistics:
HEALTH: {player.health}
STRENGTH: {player.strength}
DEFENSE:{player.defense}
SPEED:{player.speed}";
    }
}
