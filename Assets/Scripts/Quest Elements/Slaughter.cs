using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Slaughter", menuName = "Scriptable Objects/Slaughter")]
public class Slaughter : ScriptableObject
{
    public string objective = "Kill the targets!";
    public List<Target> targets = new();

    [System.Serializable]
    public struct Target
    {
        public int amount;
        public Creature[] specifier;
    }
}
