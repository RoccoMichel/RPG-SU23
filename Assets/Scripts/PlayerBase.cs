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
    internal Animator animator;
    private InputAction attackAction;
    private InputAction interactAction;
    [HideInInspector] public CameraController CameraController { get; private set; }

    [Header("References")]
    public PlayerInventory inventory;
    private Vector3 spawnPoint;
    [SerializeField] private Transform head;

    private void Start()
    {
        spawnPoint = transform.position;
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
            
            if (!hit.collider.TryGetComponent<Interact>(out var interactable))
            {
                Debug.LogWarning(hit.collider.name + " is on the 'Interact' layer but does not have Interact component attached.");
                return;
            }

            animator.Play(interactable.GetInteractAnimation());
            interactable.InteractAction();
        }        
    }

    protected virtual void Attack()
    {
        if (!canAttack) return;

        animator.Play("attack-melee-right");

        if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, interactDistance, entityLayer))
        {
            
            if (!hit.collider.TryGetComponent<Entity>(out var entity))
            {
                Debug.LogWarning(hit.collider.name + " is on the 'Entity' layer but does not have Entity component attached.");
                return;
            }

            entity.Damage(strength);
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
        amount -= (amount / 100 * defense); // reduce damage depending on defense 
        if (immortal) return;

        health = Mathf.Clamp(health - Mathf.Abs(amount), 0, maxHealth);
        if (health <= 0) Die("You died!");

        Instantiate(Resources.Load("Effects/Basic Damage"), transform.position, Quaternion.identity);
    }
    public override void Die()
    {
        animator.Play("die");
        Freeze(true);
        StartCoroutine(Freeze(false, 1));
        transform.localPosition = spawnPoint;
        GameDirector.Instance.QuestFail();
        health = maxHealth;
    }

    public void Die(string deathMessage)
    {
        Die();
        GameDirector.Instance.canvasManager.NewAlert(deathMessage, CanvasManager.AlertStyles.QuestElement);
    }

    public System.Collections.IEnumerator Freeze(bool state, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        Freeze(state);
        yield break;
    }

    public void IncreaseStrength(int amount)
    {
        strength += amount;
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    public void IncreaseSpeed(int amount)
    {
        speed += amount;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        Heal(int.MaxValue);
    }
}