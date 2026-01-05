using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Quests/Dialog Element")]
public class Dialog : ScriptableObject
{
    public bool freezePlayer = true;
    public bool setCamera = false;
    public Vector3 CameraPosition;
    public Vector3 CameraRotation;
    [Space(30)]
    public Conversation[] conversation;

    [System.Serializable]
    public struct Conversation
    {
        public Sprite speakerPortrait;
        public string title;
        [TextArea] public List<string> lines;
    }
}
