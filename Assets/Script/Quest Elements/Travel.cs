using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Travel", menuName = "Scriptable Objects/Travel")]
public class Travel : ScriptableObject
{
    public string actionName;
    public bool showDistance;
    public float missForgiveness = 3f;
    public List<Vector3> checkpoints;
    public Vector3 destination;
}
