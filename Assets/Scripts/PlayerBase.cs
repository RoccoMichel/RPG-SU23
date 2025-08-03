using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : Entity
{
    [Header("Player Attributes")]
    public float strength = 1;
    public float defense = 1;
    public float speed = 3;
    public bool canAttack = true;
    public bool canMove = true;

    private InputAction attackAction;
    [HideInInspector] public CameraController CameraController { get; private set; }

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        CameraController = Camera.main.GetComponent<CameraController>();
    }
    private void Update()
    {
        if (attackAction.WasPressedThisFrame()) Attack();
    }

    protected virtual void Attack()
    {
        if (!canAttack) return;

        // logic
    }

    /// <param name="state">True: Freeze | False: Unfreeze</param>
    public void Freeze(bool state)
    {
        canAttack = !state;
        canMove = !state;
        CameraController.locked = state;
    }

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
