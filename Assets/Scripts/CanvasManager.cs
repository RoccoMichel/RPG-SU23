using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject playerHealth;
    [HideInInspector] public DialogManager dialog;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public TMP_Text objectiveDisplay;
    [HideInInspector] public Map map;
    [HideInInspector] public GameObject cursorBoundUI;

    private List<Alert> alertQueue = new();
    private List<GameObject> notificationsQueue = new();

    private ConfirmationManager confirmationManager;
    private InputAction mapAction;
    private InputAction inventoryAction;
    private PlayerBase player;

    private struct Alert
    {
        public string message;
        public AlertStyles style;
    }

    private void Start()
    {
        mapAction = InputSystem.actions.FindAction("Map");
        inventoryAction = InputSystem.actions.FindAction("Inventory");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
    }

    private void Update()
    {
        if (mapAction.WasPressedThisFrame()) ToggleMap();
        if (inventoryAction.WasPressedThisFrame()) ToggleInventory();
    }

    // Objective Display Related
    public void SetObjective(string name)
    {
        if (objectiveDisplay == null) objectiveDisplay = InstantiateObjectiveDisplay();
        objectiveDisplay.text = "CURRENT OBJECTIVE\n" + name;
    }

    public void SetObjective(string name, bool highlightUI)
    {
        SetObjective(name);
        if (!highlightUI) return;

        Destroy(Instantiate(Resources.Load("UI/Objective Highlight"), objectiveDisplay.transform), 1f);
    }

    public void ClearObjective()
    {
        if (objectiveDisplay == null) return;

        objectiveDisplay.text = string.Empty;
    }

    private TMP_Text InstantiateObjectiveDisplay()
    {
        return Instantiate(Resources.Load("UI/Objective Display"), transform).GetComponent<TMP_Text>();
    }

    // Inventory Related
    public void ToggleInventory()
    {
        if (inventory == null) inventory = InstantiateInventory();
        else inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        player.Freeze(inventory.gameObject.activeSelf);

        if (objectiveDisplay != null) objectiveDisplay.gameObject.SetActive(!inventory.gameObject.activeSelf);
        if (playerHealth != null) playerHealth.SetActive(!inventory.gameObject.activeSelf);
        if (map != null && map.gameObject.activeSelf) map.gameObject.SetActive(false);
    }
    private Inventory InstantiateInventory()
    {
        Inventory newInventory = Instantiate(Resources.Load("UI/Inventory"), gameObject.transform).GetComponent<Inventory>();
        return newInventory;
    }

    // Map Related
    public void ToggleMap()
    {
        if (map == null) map = InstantiateMap();
        map.gameObject.SetActive(!map.gameObject.activeSelf);
        player.Freeze(map.gameObject.activeSelf);

        if (inventory != null && inventory.gameObject.activeSelf) inventory.gameObject.SetActive(false);
    }
    private Map InstantiateMap()
    {
        Map newMap = Instantiate(Resources.Load("UI/Map"), gameObject.transform).GetComponent<Map>();
        newMap.gameObject.SetActive(false);
        return newMap;
    }

    // Notification Related
    public void Notification(string message)
    {
        GameObject newNotification = Instantiate((GameObject)Resources.Load("UI/Notification"), transform);
        newNotification.GetComponent<TMP_Text>().text = message;
        notificationsQueue.Add(newNotification);

        if (notificationsQueue.Count <= 1) StartCoroutine(Notifications());
    }

    public void ClearNotifications()
    {
        foreach (GameObject notifications in notificationsQueue) { Destroy(notifications); }
        notificationsQueue.Clear();
    }

    private IEnumerator Notifications()
    {
        int maxNotifications = 4;
        int distance = 60;
        // lifetime is on prefab component

        yield return new WaitForEndOfFrame();

        while (notificationsQueue.Count > 0)
        {
            for (int i = 0; i < notificationsQueue.Count; i++)
            {
                RectTransform notification;
                try { notification = notificationsQueue[i].GetComponent<RectTransform>(); }
                catch
                {
                    notificationsQueue.RemoveAt(i);
                    if (notificationsQueue.Count == 0) break;
                    else i--;

                    continue;
                }

                Vector2 targetPosition;
                targetPosition.x = i >= maxNotifications ? 500 : 0;
                targetPosition.y = i * distance;

                notification.anchoredPosition = targetPosition;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    // Confirmation Window Related
    public void NewConfirmation(string query)
    {
        if (confirmationManager != null) Destroy(confirmationManager.gameObject);
        confirmationManager = Instantiate(Resources.Load("UI/Confirmation Window"), gameObject.transform).GetComponent<ConfirmationManager>();
        confirmationManager.SetValues(query);
    }
    public void NewConfirmation(string query, string warning)
    {
        if (confirmationManager != null) Destroy(confirmationManager.gameObject);
        confirmationManager = Instantiate(Resources.Load("UI/Confirmation Window"), gameObject.transform).GetComponent<ConfirmationManager>();
        confirmationManager.SetValues(query, warning);
    }

    // Alert Related
    public enum AlertStyles { Quest, QuestElement, Discover, Blimp } // more styles?
    public void NewAlert(string message, AlertStyles style)
    {
        Alert newAlert = new() { message = message, style = style };
        alertQueue.Add(newAlert);

        if (alertQueue.Count <= 1) StartCoroutine(Alerts());
    }

    private IEnumerator Alerts()
    {
        float displayDurationSeconds = 1f;
        float fadeOutDurationSeconds = 1.5f;

        while (alertQueue.Count > 0)
        {
            TMP_Text alertDisplay = Instantiate(Resources.Load($"UI/Alert {alertQueue[0].style}"), gameObject.transform).GetComponent<TMP_Text>();

            yield return new WaitForEndOfFrame();
            alertDisplay.text = alertQueue[0].message;
            alertDisplay.CrossFadeAlpha(1, 0, true);

            yield return new WaitForSeconds(displayDurationSeconds);
            alertDisplay.CrossFadeAlpha(0, Mathf.Clamp(fadeOutDurationSeconds, 1, float.MaxValue), true);

            yield return new WaitForSeconds(fadeOutDurationSeconds);
            alertQueue.RemoveAt(0);
            Destroy(alertDisplay.gameObject);
        }

        
        yield return null;
    }

    // CursorBoundUIElement Related
    public void NewCursorBound()
    {
        if (cursorBoundUI != null) Destroy(cursorBoundUI);

        Instantiate(Resources.Load("UI/CursorBoundUI"), gameObject.transform);
    }
    public void NewTooltip(string title, string description)
    {
        if (cursorBoundUI != null) Destroy(cursorBoundUI);

        Instantiate(Resources.Load("UI/Tooltip"), gameObject.transform).GetComponent<Tooltip>().GenerateTooltip(title, description);
    }

    // Dialog Related
    public void NewMessage(string[] contents)
    {
        if (dialog == null) dialog = InstantiateDialogPrefab();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewMessage(contents);
    }
    public void NewMessage(string[] contents, string speaker)
    {
        if (dialog == null) dialog = InstantiateDialogPrefab();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewMessage(contents, speaker);
    }
    public void NewMessage(string[] contents, string speaker, Sprite image)
    {
        if (dialog == null) dialog = InstantiateDialogPrefab();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewMessage(contents, speaker, image);
    }
    public void NewDialog(Dialog data)
    {
        if (dialog == null) dialog = InstantiateDialogPrefab();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(data);
    }
    private DialogManager InstantiateDialogPrefab()
    {
        DialogManager newDialog = Instantiate(Resources.Load("UI/Dialog System"), gameObject.transform).GetComponent<DialogManager>();
        newDialog.canvasManager = this;
        return newDialog;
    }
}
