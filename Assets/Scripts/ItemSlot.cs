using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public bool tooltip = true;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text counter;
    private GameDirector director;
    [SerializeField] private Item item;

    public void Set(PlayerInventory.ItemSlot slotData)
    {
        item = slotData.item;
        icon.sprite = item.icon;
        counter.text = slotData.amount.ToString();
    }

    public void Set(PlayerInventory.ItemSlot slotData, bool hasTooltip)
    {
        Set(slotData);
        tooltip = hasTooltip;
    }

    public void InstantiateTooltip()
    {
        if (!tooltip || item == null) return;

        if (director == null) director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        director.canvasManager.NewTooltip(item.itemName, item.description);
    }

    public void DestroyTooltip()
    {
        if (director == null) director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        if (!tooltip || director.canvasManager.cursorBoundUI == null) return;

        Destroy(director.canvasManager.cursorBoundUI);
    }
}
