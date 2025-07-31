using UnityEngine;

public class PlayerBase : Entity
{
    [Header("Player Attributes")]
    public float speed;
    
    public override void Die()
    {
        print("Player died!");
    }
}
