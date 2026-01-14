using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent events;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) events.Invoke();
    }
}
