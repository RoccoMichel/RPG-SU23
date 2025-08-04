using UnityEngine;

public class Statistics : MonoBehaviour
{
    public float MaxHealth
    {
        get
        {
            if (player == null)
            {
                try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
                catch { return 0; } 
            }
            return player.maxHealth;
        }
    }
    public float Speed
    {
        get
        {
            if (player == null)
            {
                try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
                catch { return 0; }
            }
            return player.speed;
        }
    }
    public float Strength
    {
        get
        {
            if (player == null)
            {
                try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
                catch { return 0; }
            }
            return player.strength;
        }
    }
    public float Defense
    {
        get
        {
            if (player == null)
            {
                try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
                catch { return 0; }
            }
            return player.defense;
        }
    }
    private PlayerBase player;
}

