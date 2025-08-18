using UnityEngine;

public class PickUp : MonoBehaviour
{
    public KeyCode key = KeyCode.J; // TEMP

    [Header("On Collect:")]
    public Item type;
    public int amount;
    public bool destroy = true;

    private GameDirector director;

    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key)) Collect(destroy);
    }

    public virtual void Collect(bool destroy)
    {
        director.player.inventory.AddItem(type, amount);

        if (destroy) Destroy(gameObject);
    }
}
