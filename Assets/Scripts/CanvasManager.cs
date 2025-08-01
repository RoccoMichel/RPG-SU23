using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    [HideInInspector] public DialogManager dialog;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Map map;
    private InputAction mapAction;
    private InputAction inventoryAction;
    private PlayerBase player;

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

        player.cameraController.locked = inventory.gameObject.activeSelf;
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

        if (player.frozen) player.Unfreeze();
        else player.Freeze();
    }
    private Map InstantiateMap()
    {
        Map newMap = Instantiate(Resources.Load("UI/Map"), gameObject.transform).GetComponent<Map>();
        newMap.gameObject.SetActive(false);
        return newMap;
    }

    // Quest Related
    public void AssignQuestGiver(QuestGiver questGiver)
    {
        dialog.questGiver = questGiver;
    }

    // Dialog Related
    public void NewDialog(string[] contents)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents);
    }
    public void NewDialog(string[] contents, string title)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents, title);
    }
    public void NewDialog(string[] contents, string title, Sprite image)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents, title, image);
    }
    private DialogManager InstantiateDialog()
    {
        DialogManager newDialog = Instantiate(Resources.Load("UI/DialogSystem"), gameObject.transform).GetComponent<DialogManager>();
        newDialog.canvasManager = this;
        return newDialog;
    }
}
