using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public bool tooltip = true;
    [SerializeField] private Item item;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Inventory inventoryUI;

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

    public void UpdateTooltip()
    {
        if (!tooltip || item == null) return;

        if (inventoryUI == null) inventoryUI = gameObject.GetComponentInParent<Inventory>();
        inventoryUI.UpdateItemTooltip(item);
    }
}
