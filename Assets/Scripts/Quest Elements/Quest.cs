using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName = "Unnamed Quest";
    [TextArea] public string description;
    public bool mainQuest = false;
    public QuestElement[] elements;
    [TextArea, SerializeField] private string devNote;
    public enum ElementsTypes { Dialog, Travel, Fetch, Slaughter, Reward };

    [System.Serializable]
    public struct QuestElement
    {
        public ElementsTypes type;
        public ScriptableObject data;
    }
}
