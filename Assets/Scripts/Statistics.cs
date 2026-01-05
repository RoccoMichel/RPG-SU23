using System.Collections.Generic;
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

    public List<CreatureClass> defeatedCreatures = new();

    public struct CreatureClass
    {
        public Creature entity;
        public int count;
        public int highestLevel;
    }

    public void AddKill(Creature creature)
    {
        for (int i = 0; i < defeatedCreatures.Count; i++)
        {
            if (defeatedCreatures[i].entity == creature)
            {
                var updatedClass = defeatedCreatures[i];
                updatedClass.count++;
                if (creature.level > updatedClass.highestLevel) updatedClass.highestLevel = creature.level;

                defeatedCreatures[i] = updatedClass;
                return;
            }
        }

        CreatureClass newClass = new()
        {
            entity = creature,
            count = 1,
            highestLevel = creature.level
        };
        defeatedCreatures.Add(newClass);
    }
}

