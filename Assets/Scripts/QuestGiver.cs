using System.Collections;
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

    private Animator animator;
    private GameDirector director;

    [System.Serializable]
    public struct QuestElement
    {
        public Quest quest;
        public UnityEvent OnComplete;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    public void Interact()
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

        StartCoroutine(DelayedCompletion());
    }

    private IEnumerator DelayedCompletion()
    {
        yield return new WaitForEndOfFrame();
        Quests[index - 1].OnComplete.Invoke();

        yield break;
    }

    /// <summary>
    /// Makes every quest available again, by setting index to zero.
    /// </summary>
    public void ResetQuests()
    {
        index = 0; 
        completed = false;
    }
    /// <summary>
    /// Removes a single instance of the specified item from the player's inventory.
    /// Can not be more than 1 because Unity Inspector does not support multiple parameters.
    /// </summary>
    public void RemoveItemFromPlayer(Item item)
    {
        director.player.inventory.TryRemoveItem(item, 1, true);
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
}
