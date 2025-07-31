using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool freeze;
    private PlayerBase playerStats;
    private CharacterController controller;
    private InputAction moveAction;
    private Vector2 moveValue;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        try { playerStats = GetComponent<PlayerBase>(); }
        catch { Debug.LogWarning($"{gameObject.name} is missing 'PlayerBase' while having 'PlayerMovement' assigned as component!"); Debug.Break(); }
        
        try { controller = GetComponent<CharacterController>(); }
        catch { Debug.LogWarning($"{gameObject.name} is missing 'CharacterController' while having 'PlayerMovement' assigned as component!"); Debug.Break(); }

    }

    private void Update()
    {
        if (freeze) return;

        moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move = playerStats.speed * (transform.right * moveValue.x + transform.forward * moveValue.y);
        controller.Move(Time.deltaTime * move);

    }

}
