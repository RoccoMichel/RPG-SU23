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
    public bool respawn = true;

    [Header("Loot")]
    public List<LootItem> lootTable = new();

    [SerializeField] private float healthBarOffset;
    private Vector3 spawnPoint;
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

        agent.speed = speed;
        healthBar = InstantiateHealthBar();

        if (respawn) director.respawnCreatures.AddListener(Respawn);
        spawnPoint = transform.position;

        base.OnStart();
    }

    private void Update()
    {
        if (health <= 0) return; // dead but not destroyed for respawn purposes

        distanceToPlayer = Vector3.Distance(transform.position, director.player.transform.position);
        if (healthBar != null) healthBar.gameObject.SetActive(distanceToPlayer < 8f); // Hide health bar after 8 meters
        else healthBar = InstantiateHealthBar();
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
        Vector3 offset = new Vector3(0, healthBarOffset, 0);
        HealthBar newHealthBar = Instantiate(Resources.Load("HealthBar"), transform.position + offset, Quaternion.identity, transform).GetComponent<HealthBar>();
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

        if (respawn)
        {
            if (healthBar != null) Destroy(healthBar.gameObject);
            agent.enabled = false;
            transform.position = new Vector3(0, -1234, 0);

            return; // skip base.Die()

        }

        base.Die();
    }

    public void Respawn()
    {
        health = maxHealth;
        transform.position = spawnPoint;

        agent.enabled = true;
        agent.Warp(transform.position); // fix NavMeshAgent
    }
}