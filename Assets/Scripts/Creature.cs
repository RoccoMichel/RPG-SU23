using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class Creature : Entity
{
    [Header("Creature Attributes")]
    public int level = 1;
    public float strength = 1;
    public float defense;
    public float speed = 1;
    private HealthBar healthBar;
    private NavMeshAgent agent;

    private void Start()
    {
        try
        {
            agent = GetComponent<NavMeshAgent>();
        }
        finally { }
    }

    public override void Damage(float amount)
    {
        if (healthBar == null) healthBar = InstantiateHealthBar();
        healthBar.Damage(amount);
        base.Damage(amount);
    }
    public override void Heal(float amount)
    {
        if (healthBar == null) healthBar = InstantiateHealthBar();
        healthBar.Heal(amount);
        base.Heal(amount);
    }

    protected virtual HealthBar InstantiateHealthBar()
    {
        HealthBar newHealthBar = Instantiate(Resources.Load("HealthBar"), transform).GetComponent<HealthBar>();
        newHealthBar.Set(0, maxHealth, health, level, identity);
        return newHealthBar;
    }
}
