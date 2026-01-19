using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool open;
    [SerializeField] private Transform link;
    [SerializeField] private Transform hinge;
    private float startingAngle = 0;

    private void Start()
    {
        startingAngle = hinge.localRotation.eulerAngles.y;
    }

    public void Interact()
    {
        open = !open;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (open && other.CompareTag("Player"))
        {
            GameDirector.Instance.player.Freeze(true);
            other.transform.position = link.position;
            GameDirector.Instance.player.CameraController.Reset();
            StartCoroutine(GameDirector.Instance.player.Freeze(false, 0.1f));

            open = false;
        }
    }

    private void Update()
    {
        hinge.localRotation = Quaternion.Euler(hinge.localRotation.eulerAngles.x, Mathf.LerpAngle(hinge.localRotation.eulerAngles.y, open ? startingAngle-105 : startingAngle, 3f * Time.deltaTime), hinge.rotation.eulerAngles.z);
    }
}
