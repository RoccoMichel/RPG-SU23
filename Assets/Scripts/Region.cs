using System.Collections;
using UnityEngine;

public class Region : MonoBehaviour
{
    private bool newDiscovery = true;
    private bool notifyPlayer = true;
    private GameDirector director;

    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !notifyPlayer) return;

        if (newDiscovery) director.canvasManager.NewAlert("New Region\n discovered:");
        director.canvasManager.NewAlert(name);

        StartCoroutine(PauseRegionNotification(30));
        newDiscovery = false;
    }

    private IEnumerator PauseRegionNotification(int seconds)
    {
        notifyPlayer = false;

        yield return new WaitForSeconds(seconds);

        notifyPlayer = true;
        yield return null;
    }

    private void Reset()
    {
        Debug.Log("Reminder: Region name is GameObject name.");
    }
}
