using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInventory), typeof(Animator))]
public class PlayerBase : Entity
{
    [Header("Player Attributes")]
    public float strength = 1;
    public float defense = 1;
    public float speed = 3;
    public bool canAttack = true;
    public bool canMove = true;
    private Animator animator;
    private InputAction attackAction;
    private InputAction interactAction;
    [HideInInspector] public CameraController CameraController { get; private set; }

    [Header("Inventory")]
    public PlayerInventory inventory;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        attackAction = InputSystem.actions.FindAction("Attack");
        interactAction = InputSystem.actions.FindAction("Interact");
        CameraController = Camera.main.GetComponent<CameraController>();
    }
    private void Update()
    {
        if (attackAction.WasPressedThisFrame()) Attack();
        if (interactAction.WasPressedThisFrame()) Interact();
    }

    public void Interact()
    {
        if (!canMove) return;
        animator.Play("pick-up");
    }

    protected virtual void Attack()
    {
        if (!canAttack) return;
        animator.Play("attack-melee-right");
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