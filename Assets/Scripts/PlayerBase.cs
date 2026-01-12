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
    public float interactDistance = 1.5f;
    public LayerMask interactLayer;
    public LayerMask entityLayer;
    private Animator animator;
    private InputAction attackAction;
    private InputAction interactAction;
    [HideInInspector] public CameraController CameraController { get; private set; }

    [Header("References")]
    public PlayerInventory inventory;
    [SerializeField] private Transform head;

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

        if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            Interact interactable = hit.collider.GetComponent<Interact>();

            if (interactable == null)
            {
                Debug.LogWarning(hit.collider.name + " is on the 'Interact' layer but does not have Interact component attached.");
                return;
            }

            interactable.InteractAction();

            animator.Play(interactable.GetInteractAnimation());
        }        
    }

    protected virtual void Attack()
    {
        if (!canAttack) return;

        if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, interactDistance, entityLayer))
        {
            Entity entity = hit.collider.GetComponent<Entity>();

            if (entity == null)
            {
                Debug.LogWarning(hit.collider.name + " is on the 'Entity' layer but does not have Entity component attached.");
                return;
            }

            entity.Damage(strength);

            animator.Play("attack-melee-right");
        }
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