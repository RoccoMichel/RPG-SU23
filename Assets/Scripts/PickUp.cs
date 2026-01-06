using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    [Header("On Collect:")]
    public Item type;
    public int amount;
    public bool destroy = true;

    private bool canPickUp;
    private GameDirector director;
    private InputAction interactAction;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    private void Update()
    {
        if (canPickUp && interactAction.WasPressedThisFrame()) Collect(destroy);
    }

    public virtual void Collect(bool destroy)
    {
        director.player.inventory.AddItem(type, amount, true);

        if (destroy) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canPickUp = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canPickUp = false;
    }
}
