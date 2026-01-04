using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Quests/Reward Element")]
public class Reward : ScriptableObject
{
    public bool notification = true;
    public Items[] reward;
    [System.Serializable]
    public struct Items
    {
        public Item item;
        public int amount;
    }
}
