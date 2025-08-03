using UnityEngine;

public class TravelRoute : MonoBehaviour
{
    private GameDirector director;
    private SphereCollider finishCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        director.QuestAdvance();
        Destroy(gameObject);
    }

    public void SetTravelRoute(Travel data, GameDirector director)
    {
        finishCollider = GetComponent<SphereCollider>();
        finishCollider.radius = data.missForgiveness;
        transform.position = data.destination;
        this.director= director;
    }
}
