using UnityEngine;

[CreateAssetMenu(fileName = "New Slaughter", menuName = "Quests/Slaughter Element")]
public class Slaughter : ScriptableObject
{
    public Target[] targets;

    [System.Serializable]
    public struct Target
    {
        public int amount;
        public Creature creature;
    }
}
