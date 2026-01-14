using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PickUp : MonoBehaviour
{
    [Header("On Collect:")]
    public Item type;
    public int amount;
    public bool respawn = true;
    [SerializeField] private GameObject model;
    private CapsuleCollider trigger;
    private GameDirector director;

    private void Start()
    {
        trigger = GetComponent<CapsuleCollider>();
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        director.respawnPickUps.AddListener(Respawn);
    }

    private void Respawn()
    {
        model.SetActive(true);
        trigger.enabled = true;
    }

    public virtual void Collect(bool destroy)
    {
        trigger.enabled = false;
        model.SetActive(false);

        director.player.inventory.AddItem(type, amount, true);

        if (destroy) Destroy(gameObject);
    }

    public void Interact() => Collect(!respawn);
}
