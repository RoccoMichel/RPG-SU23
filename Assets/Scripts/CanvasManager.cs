using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [HideInInspector] public DialogManager dialog;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Map map;
    private InputAction mapAction;
    private InputAction inventoryAction;
    private PlayerBase player;
    [SerializeField] private List<string> alertQueue = new();

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

    // Inventory Related
    public void ToggleInventory()
    {
        if (inventory == null) inventory = InstantiateInventory();
        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);

        player.CameraController.locked = inventory.gameObject.activeSelf;
    }
    private Inventory InstantiateInventory()
    {
        Inventory newInventory = Instantiate(Resources.Load("UI/Inventory"), gameObject.transform).GetComponent<Inventory>();
        newInventory.gameObject.SetActive(false);
        return newInventory;
    }

    // Map Related
    public void ToggleMap()
    {
        if (map == null) map = InstantiateMap();

        map.gameObject.SetActive(!map.gameObject.activeSelf);
        player.Freeze(map.gameObject.activeSelf);
    }
    private Map InstantiateMap()
    {
        Map newMap = Instantiate(Resources.Load("UI/Map"), gameObject.transform).GetComponent<Map>();
        newMap.gameObject.SetActive(false);
        return newMap;
    }

    // Alert Related
    public void NewAlert(string displayMessage) // ADD: different alert styles
    {
        alertQueue.Add(displayMessage);
        if (alertQueue.Count <= 1) StartCoroutine(Alert());
    }

    private IEnumerator Alert()
    {
        float displayDurationSeconds = 1f;
        float fadeOutDurationSeconds = 1.5f;
        TMP_Text alertDisplay = Instantiate(Resources.Load("UI/Alert"), gameObject.transform).GetComponent<TMP_Text>();

        yield return new WaitForEndOfFrame();

        while (alertQueue.Count > 0)
        {
            alertDisplay.text = alertQueue[0];
            alertDisplay.CrossFadeAlpha(1, 0, true);

            yield return new WaitForSeconds(displayDurationSeconds);
            alertDisplay.CrossFadeAlpha(0, Mathf.Clamp(fadeOutDurationSeconds, 1, float.MaxValue), true);

            yield return new WaitForSeconds(fadeOutDurationSeconds);
            alertQueue.RemoveAt(0);
        }

        Destroy(alertDisplay.gameObject);
        yield return null;
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
        DialogManager newDialog = Instantiate(Resources.Load("UI/DialogSystem"), gameObject.transform).GetComponent<DialogManager>();
        newDialog.canvasManager = this;
        return newDialog;
    }
}
