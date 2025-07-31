using UnityEngine;

public class PlayerBase : Entity
{
    [Header("Player Attributes")]
    public float strength = 1;
    public float defense = 1;
    public float speed = 3;

    public override void Heal(float amount)
    {
        base.Heal(amount);
        Instantiate(Resources.Load("Effects/Heal"), transform);
    }
    public override void Damage(float amount)
    {
        base.Damage(amount);
        Instantiate(Resources.Load("Effects/Basic Damage"), transform.position, Quaternion.identity);
    }
    public override void Die()
    {
        print("Player died!");
    }
}
