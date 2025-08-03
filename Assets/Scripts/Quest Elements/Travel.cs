using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Travel", menuName = "Scriptable Objects/Travel")]
public class Travel : ScriptableObject
{
    public string objective;
    public bool showDistance;
    public bool showWaypoints;
    public float missForgiveness = 3f;
    public List<Vector3> waypoints;
    public Vector3 destination;
}
