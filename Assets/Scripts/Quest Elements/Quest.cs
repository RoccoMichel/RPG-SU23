using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName = "Unnamed Quest";
    [TextArea] public string description;
    public List<QuestElement> elements;
    public enum ElementsTypes { Dialog, Travel, Fetch, Slaughter };

    [System.Serializable]
    public struct QuestElement
    {
        public ElementsTypes type;
        public ScriptableObject data;
    }
}
