using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName = "Unnamed Item";
    [TextArea] public string description = "Super Exciting!";
    public int stackSize = 100;
    public Sprite icon;
    public Types type;
    [Tooltip("The effect of the item type (i.e. damage, heal, defense, etc...)")]
    public float modifier;

    public enum Types { Regular, Food, Weapon, Armor }
}
