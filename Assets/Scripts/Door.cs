using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private Transform link;
    private Animator animator;
    
    public void Interact()
    {
        open = !open;

        if (animator == null) animator = GetComponent<Animator>();
        animator.Play(!open ? "Open" : "Close");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (open && other.CompareTag("Player"))
        {
            other.transform.SetPositionAndRotation(link.position, link.rotation);
        }
    }
}
