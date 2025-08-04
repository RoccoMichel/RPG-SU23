using UnityEngine;

public class Map : MonoBehaviour
{
    //!\\ World center needs to be at 0,0,0 //!\\
    public Vector2 zoomClamp = new (0.5f, 2);
    [Tooltip("Position multiplier, map is (800x800 units)")]
    public float mapScale = 2f;
    public RectTransform map;
    public RectTransform playerMap;
    private Transform playerWorld;
    private float zoomLevel = 1f;
    public const float ZOOM_SENSITIVITY = 0.1f;

    private void Start()
    {
        playerWorld = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        playerMap.anchoredPosition = new Vector3(playerWorld.position.x, playerWorld.position.z) * mapScale;
        map.localScale = new Vector3(zoomLevel, zoomLevel);
        zoomLevel = Mathf.Clamp(zoomLevel + Input.mouseScrollDelta.y * ZOOM_SENSITIVITY, zoomClamp.x, zoomClamp.y);
    }
}
