using UnityEngine;

[CreateAssetMenu(fileName = "New Fetch", menuName = "Quests/Fetch Element")]
public class Fetch : ScriptableObject
{
    public Target[] FetchList;
    
    [System.Serializable]
    public struct Target
    {
        public Item type;
        public int amount;
    }
}
