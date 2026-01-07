using UnityEngine;
using UnityEngine.Events;

public class Interact : MonoBehaviour
{
    public InteractAnimations interactAnimation = InteractAnimations.interact_right;
    public UnityEvent onInteract;

    public enum InteractAnimations
    {
        interact_left,
        interact_right,
        pick_up,
    };

    public void InteractAction()
    {
        onInteract.Invoke();
    }

    public string GetInteractAnimation()
    {
        return interactAnimation.ToString().Replace('_', '-');
    }
}
