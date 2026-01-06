using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class QuestGiver : Entity
{
    [Header("Quest Giver Attributes")]
    [SerializeField] private int index;
    public QuestElement[] Quests;
    public bool completed;
    private bool interactable;

    private Animator animator;
    private GameDirector director;
    private InputAction interactAction;

    [System.Serializable]
    public struct QuestElement
    {
        public Quest quest;
        public UnityEvent OnComplete;
    }

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
                director.canvasManager.NewMessage(new string[] { "You have already completed all my quests!", }, "Quest-giver");
                return;
            }
            else
            {
                if (director.ActiveQuest == Quests[index].quest) return;

                string warning = director.InQuest() ? 
                    $"You are already on a quest ({director.ActiveQuest.questName})!\nAccepting this will discard your current quest and its progress." 
                    : string.Empty;

                director.RequestConfirmation($"Accept new quest:\n{Quests[index].quest.questName}", warning);  

                director.confirmationEvent.AddListener(QuestStart);
                director.rejectionEvent.AddListener(QuestCancel);
            }
        }
    }

    private void QuestCancel()
    {
        animator.Play("emote-no");
        director.canvasManager.Notification(identity + ": No hard feelings");
        director.canvasManager.NewAlert($"QUEST CANCELLED", CanvasManager.AlertStyles.QuestElement);
    }
    private void QuestStart()
    {
        if (Quests == null)
        {
            Debug.LogWarning($"{identity} ({name}) has no quest to give!");
            return;
        }

        animator.Play("interact-right");
        director.QuestStart(Quests[index].quest, this);
    }

    public void QuestComplete()
    {
        animator.Play("emote-yes");

        index++;
        if (index >= Quests.Count()) completed = true;

        // Important Invoke is Last!
        Quests[index-1].OnComplete.Invoke();

    }

    /// <summary>
    /// Makes every quest available again, by setting index to zero.
    /// </summary>
    public void ResetQuests()
    {
        index = 0; 
        completed = false;
    }
    public void Relocate(Vector3 newLocation)
    {
        transform.position = newLocation;
    }

    public override void Die()
    {
        animator.Play("die");
        base.Die();
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
