using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Attributes")]
    public bool locked;
    public float fov = 60f;
    public float distance = 5;
    public Vector3 orbitOffset;
    public Vector2 verticalLimit = new Vector2 (0, 50); //x: lowest, y: highest
    public Vector2 sensitivity = Vector2.one;
    /*[SerializeField]*/ private Vector2 rotationRaw;
    /*[SerializeField]*/ private Vector2 rotationFree;
    /*[SerializeField]*/ private Vector3 rotation;

    [Header("References")]
    [SerializeField] private Transform orbit;
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    private InputAction lookAction;
    private InputAction freelookAction;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        lookAction = InputSystem.actions.FindAction("Look");
        freelookAction = InputSystem.actions.FindAction("Freelook");

        if (player == null)
        {
            try { player = GameObject.FindGameObjectWithTag("Player").transform; } finally { };
        }
        if (orbit == null)
        {
            try { orbit = gameObject.transform.parent.transform; } finally { };
        }
    }

    private void Update()
    {
        if (locked) return;

        fov = Mathf.Clamp(fov, 0.1f, 179);
        cam.fieldOfView = fov;
        transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -distance);

        rotationRaw = lookAction.ReadValue<Vector2>();
        if (freelookAction.WasPressedThisFrame()) rotationFree = rotation;        

        // Freelook (sensitivity.y ONLY)
        if (freelookAction.inProgress)
        {
            rotationFree.x += rotationRaw.x * sensitivity.x;
            rotationFree.y = Mathf.Clamp(rotationFree.y + (rotationRaw.y * sensitivity.y), -verticalLimit.y, verticalLimit.x);
            if (rotationFree.y > 360) rotationFree.y -= 360;
            else if (rotationFree.y < -360) rotationFree.y += 360;

            orbit.transform.rotation = Quaternion.Euler(-rotationFree.y, rotationFree.x, 0f);
            orbit.transform.localPosition = Vector3.zero;
        }
        // Vertical Look Horizontal Player Rotation (sensitivity.x & sensitivity.y)
        else
        {
            rotation.x += rotationRaw.x * sensitivity.x;
            rotation.y = Mathf.Clamp(rotation.y + (rotationRaw.y * sensitivity.y), -verticalLimit.y, verticalLimit.x);
            if (rotation.x > 360) rotation.x -= 360;
            else if (rotation.x < -360) rotation.x += 360;

            orbit.transform.localRotation = Quaternion.Euler(-rotation.y, 0, 0f);
            player.transform.rotation = Quaternion.Euler(player.eulerAngles.x, rotation.x, player.eulerAngles.z);
            orbit.transform.localPosition = orbitOffset;
        }
    }

    public void ResetRotation()
    {
        rotationFree = Vector3.zero;
        rotationRaw = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
