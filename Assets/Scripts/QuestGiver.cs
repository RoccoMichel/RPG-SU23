using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class QuestGiver : Entity
{
    [Header("Quest Giver Attributes")]
    [SerializeField] private int index;
    public Quest[] Quests;
    public bool completed;
    private bool interactable;

    private Animator animator;
    private GameDirector director;
    private InputAction interactAction;

    private void Start()
    {
        animator = GetComponent<Animator>();
        interactAction = InputSystem.actions.FindAction("Interact");
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    private void Update()
    {
        if (interactAction.WasPressedThisFrame() && interactable)
        {
            if (completed)
            {
                director.canvasManager.NewMessage(new string[] { "You have already completed all my quests!", });
                return;
            }
            else if (director.InQuest()) return;

            QuestStart();
        }
    }

    private void QuestStart()
    {
        if (Quests == null)
        {
            Debug.LogWarning($"{identity} ({name}) has no quest to give!");
            return;
        }

        animator.Play("interact-right");
        director.QuestStart(Quests[index], this);
    }

    public void QuestComplete()
    {
        index++;
        if (index >= Quests.Count()) completed = true;

        animator.Play("emote-yes");
    }

    public void Move(Vector3 newLocation)
    {
        transform.position = newLocation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) interactable = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) interactable = false;
    }
}
