using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Creature), typeof(NavMeshAgent), typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Creature creature;
    public AiStates aiState;
    private Vector3 spawn;
    private Transform player;
    private Animator animator;
    public enum AiStates
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        creature = GetComponent<Creature>();
        animator = GetComponent<Animator>();
        aiState = AiStates.Idle;
        spawn = transform.position;
        agent.speed = creature.speed;

        StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        yield return new WaitForEndOfFrame();
        float nextAttackTime = Time.time;

        while (true)
        {
            if (creature.health <= 0)
            {
                yield return new WaitForSeconds(10);
                continue;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float distanceToSpawn = Vector3.Distance(transform.position, spawn);


            animator.SetFloat("Speed", agent.velocity.magnitude);
            switch (aiState)
            {
                // IDLE LOGIC
                case AiStates.Idle:
                    if (distanceToPlayer < 30f)
                    {
                        aiState = AiStates.Patrol;
                        agent.SetDestination(spawn);
                        yield return new WaitForSeconds(agent.remainingDistance/creature.speed);
                    }
                    break;

                case AiStates.Patrol:

                    Vector3 newDestination = new(
                        spawn.x + Random.Range(-10f, 10f),
                        spawn.y,
                        spawn.z + Random.Range(-10f, 10f)
                    );
                    agent.SetDestination(newDestination);
                    yield return new WaitForSeconds(agent.remainingDistance / creature.speed);

                    if (distanceToPlayer < 20f)
                    {
                        aiState = AiStates.Chase;
                    }
                    else if (distanceToPlayer > 35f)
                    {
                        aiState = AiStates.Idle;
                    }
                    break;
                
                // CHASE LOGIC
                case AiStates.Chase:
                    agent.SetDestination(player.position);
                    if (distanceToPlayer < 2.5f && nextAttackTime < Time.time)
                    {
                        aiState = AiStates.Attack;
                    }
                    else if (distanceToPlayer > 15f || distanceToSpawn > 40)
                    {
                        aiState = AiStates.Idle;
                        agent.SetDestination(spawn);
                    }
                    break;

                // ATTACK LOGIC
                case AiStates.Attack:
                    if (Random.Range(0, 2) == 0) animator.Play("attack-melee-right");
                    else animator.Play("attack-kick-right");

                    player.GetComponent<PlayerBase>().Damage(creature.strength);
                    nextAttackTime = Time.time + Random.Range(0.5f, 2f);
                    aiState = AiStates.Chase;
                    
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
