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

                director.RequestConfirmation($"Accept new Quest: {Quests[index].quest.questName}", warning);  

                director.confirmationEvent.AddListener(QuestStart);
                director.rejectionEvent.AddListener(QuestCancel);
            }
        }
    }

    private void QuestCancel()
    {
        director.canvasManager.NewMessage(new string[] { "No hard feelings", }, "Quest-giver");
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
        Quests[index].OnComplete.Invoke();

        index++;
        if (index >= Quests.Count()) completed = true;        
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
