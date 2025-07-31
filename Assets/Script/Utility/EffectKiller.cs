using UnityEngine;

public class EffectKiller : MonoBehaviour
{
    public float destroyAfter;
    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
}
