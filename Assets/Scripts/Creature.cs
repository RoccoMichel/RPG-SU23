using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
public class Creature : Entity
{
    [Header("Creature Attributes")]
    public int level = 1;
    public float strength = 1;
    public float defense;
    public float speed = 1;    

    [Header("Loot")]
    public List<LootItem> lootTable = new();

    private float distanceToPlayer;
    private HealthBar healthBar;
    private GameDirector director;
    private NavMeshAgent agent;

    [System.Serializable]
    public struct LootItem
    {
        public Item item; // or use PickUp class so it has to physically be picked 
        [Range(0f, 1f)] public float dropChance;
        public Vector2 minMaxAmount;

        public readonly int RollDrop()
        {
            float roll = Random.Range(0, 1f);
            if (roll < dropChance) return Mathf.CeilToInt(Random.Range(minMaxAmount.x, minMaxAmount.y));

            return 0; // unlucky
        }
    }

    protected override void OnStart()
    {
        agent = GetComponent<NavMeshAgent>();
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();

        healthBar = InstantiateHealthBar();

        base.OnStart();
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, director.player.transform.position);
        if (healthBar != null) healthBar.gameObject.SetActive(distanceToPlayer < 8f); // Hide health bar after 8 meters
    }

    public override void Damage(float amount)
    {
        amount = amount - (amount / 100 * defense); // reduce damage depending on defense 
        if (healthBar == null) healthBar = InstantiateHealthBar();
        healthBar.Set(0, maxHealth, health);
        base.Damage(amount);
    }
    public override void Heal(float amount)
    {
        if (healthBar == null) healthBar = InstantiateHealthBar();
        healthBar.Set(0, maxHealth, health);
        base.Heal(amount);
    }

    protected virtual HealthBar InstantiateHealthBar()
    {
        HealthBar newHealthBar = Instantiate(Resources.Load("HealthBar"), transform).GetComponent<HealthBar>();
        newHealthBar.Set(0, maxHealth, health, level, identity);
        return newHealthBar;
    }

    public override void Die()
    {
        // Report to Game Director for statistics and possible slaughter quest
        director.ReportDeath(this);

        // Drop Loot
        foreach (LootItem loot in lootTable)
        {
            director.player.inventory.AddItem(loot.item, loot.RollDrop(), true);
        }

        base.Die();
    }
}