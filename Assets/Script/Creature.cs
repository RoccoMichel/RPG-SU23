using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Creature : Entity
{
    [Header("Entity Attributes")]
    public float strength = 1;
    public float defense;
    public float speed = 1;
    private NavMeshAgent agent;

}
