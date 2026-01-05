using UnityEngine;

public class CursorBoundUI : MonoBehaviour
{
    protected float edgeThreshold = 0.7f;
    protected GameDirector director;
    protected RectTransform rect;
    private Canvas canvas;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        director.canvasManager.cursorBoundUI = gameObject;

        SetPosition();
    }

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        rect.anchoredPosition += new Vector2(Input.mousePositionDelta.x, Input.mousePositionDelta.y);

        Vector2 mousePosition = Input.mousePosition;
        //Vector2 screenSize = new(Screen.width, Screen.height);

        //bool flipX = mousePosition.x > screenSize.x * edgeThreshold;
        //bool flipY = mousePosition.y < screenSize.y * (1 - edgeThreshold);
        //rect.pivot = new(flipX ? 1f : 0f, flipY ? 0f : 1f);

        rect.pivot = new(1, 0);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPos
        );

        // Vector2.up is a temporary bandage fix
        rect.anchoredPosition = localPos + Vector2.up;

    }
}

