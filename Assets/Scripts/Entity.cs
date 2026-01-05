using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Attributes")]
    public string identity = "Unnamed Entity";
    public float health = 10;
    public float maxHealth = 10;
    public bool immortal;

    protected virtual void OnStart()
    {
        if (!immortal && health <= 0) Die();
    }

    private void Start() => OnStart();

    public virtual void Damage(float amount)
    {
        if (immortal) return;

        health = Mathf.Clamp(health - Mathf.Abs(amount), 0, maxHealth);
        if (health <= 0) Die();
    }

    public virtual void Heal(float amount)
    {
        health = Mathf.Clamp(health + Mathf.Abs(amount), 0, maxHealth);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
