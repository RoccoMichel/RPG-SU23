using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerBase playerStats;
    private CharacterController controller;
    private InputAction moveAction;
    private Vector2 moveValue;
    private Vector2 direction;
    private Vector3 velocity;
    private bool isGrounded;
    private float momentum;
    private const float GRAVITY = -9.81f;
    public float accelerationSpeed = 4;
    public float decelerationSpeed = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        controller = GetComponent<CharacterController>();

        try { playerStats = GetComponent<PlayerBase>(); }
        catch { Debug.LogWarning($"{gameObject.name} is missing 'PlayerBase' while having 'PlayerMovement' assigned as component!"); Debug.Break(); }
    }

    private void Update()
    {
        if (!playerStats.canMove) return;

        moveValue = moveAction.ReadValue<Vector2>();
        momentum = Mathf.Clamp01(moveValue == Vector2.zero ? momentum - decelerationSpeed * Time.deltaTime : momentum + accelerationSpeed * Time.deltaTime);
        direction = Vector2.Lerp(direction, moveValue, 0.015f);
                
        Vector3 move = momentum * playerStats.speed * (transform.right * direction.x + transform.forward * direction.y);
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += GRAVITY * Time.deltaTime;

        controller.Move(Time.deltaTime * move);
        controller.Move(Time.deltaTime * velocity);
    }

    public void SetSpeed(float f) => playerStats.speed = f;
}
