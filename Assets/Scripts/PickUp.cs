using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    [Header("On Collect:")]
    public Item type;
    public int amount;
    public bool destroy = true;

    private GameDirector director;

    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    public virtual void Collect(bool destroy)
    {
        director.player.inventory.AddItem(type, amount, true);

        if (destroy) Destroy(gameObject);
    }

    public void Interact() => Collect(destroy);
}
