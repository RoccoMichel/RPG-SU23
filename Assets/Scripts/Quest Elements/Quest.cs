using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName = "Unnamed Quest";
    [TextArea] public string description;
    public List<QuestElement> elements;
    public enum ElementsTypes { Dialog, Travel, Fetch, Slaughter, Reward };

    [System.Serializable]
    public struct QuestElement
    {
        public ElementsTypes type;
        public ScriptableObject data;
        public UnityEvent OnComplete;
    }
}
