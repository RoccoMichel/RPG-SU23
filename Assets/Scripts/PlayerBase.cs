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
    public bool frozen { get; private set; }

    private InputAction attackAction;
    [HideInInspector] public CameraController cameraController { get; private set; }

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    private void Update()
    {
        if (attackAction.WasPressedThisFrame() && canAttack) Attack();
    }

    protected virtual void Attack()
    {
        Debug.Log("Player Attack!");
    }

    public void Freeze()
    {
        canAttack = false;
        canMove = false;
        cameraController.locked = true;
        frozen = true;
    }

    public void Unfreeze()
    {
        canAttack = true;
        canMove = true;
        cameraController.locked = false;
        frozen = false;
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
