using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog")]
public class Dialog : ScriptableObject
{
    public bool skippable;
    public List<Conversation> conversation;

    [System.Serializable]
    public struct Conversation
    {
        public Sprite speaker;
        public string title;
        [TextArea] public List<string> lines;
    }
}
