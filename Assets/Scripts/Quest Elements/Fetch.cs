using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Fetch", menuName = "Quests/Fetch Element")]
public class Fetch : ScriptableObject
{
    public string objective;
    public Element[] FetchList;
    
    [System.Serializable]
    public struct Element
    {
        public Item type;
        public int amount;
    }
}
