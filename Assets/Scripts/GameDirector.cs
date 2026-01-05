using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Statistics))]
public class GameDirector : MonoBehaviour
{
    public bool debug;
    public static GameDirector instance;
    private InputAction debugAction;

    [Header("Quest Related")]
    public Quest ActiveQuest;
    private QuestGiver questGiver;
    [SerializeField] private int questStage;
    private List<string> slaughterTargets;
    private List<int> slaughterRemaining;

    [Header("References")]
    public CanvasManager canvasManager;
    public Statistics statistics;
    public PlayerBase player;
    public Blimp blimp;

    [HideInInspector] public UnityEvent confirmationEvent;
    [HideInInspector] public UnityEvent rejectionEvent;

    public bool InQuest() { return ActiveQuest != null; }

    private void Start()
    {
        if (instance == null ) instance = this;

        try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
        catch { Debug.LogWarning("No Player active in Scene!"); }

        if (canvasManager == null)
        {
            try { canvasManager = FindFirstObjectByType<CanvasManager>().GetComponent<CanvasManager>(); }
            catch { Debug.LogError("No CanvasManager active in Scene!"); Debug.Break(); }
        }

        if (blimp == null)
        {
            try { blimp = FindFirstObjectByType<Blimp>().GetComponent<Blimp>(); }
            catch { Debug.LogWarning("No Blimp active in Scene! Main Quests will have no effect!"); }
        }

        statistics = GetComponent<Statistics>();
        debugAction = InputSystem.actions.FindAction("Debug");
    }

    private void Update()
    {
        if (debugAction.WasPressedThisFrame()) debug = !debug;
    }

    public void ReportDeath(Creature deceased)
    {
        statistics.AddKill(deceased);
        if (slaughterTargets != null) SlaughterProgress(deceased.identity);
    }

    private void QuestLogic()
    {
        if (ActiveQuest == null) return;

        switch(ActiveQuest.elements[questStage].type)
        {
            case Quest.ElementsTypes.Dialog:
                Dialog dataDialog = ActiveQuest.elements[questStage].data as Dialog;
                if (dataDialog.freezePlayer) player.Freeze(true);
                if (dataDialog.setCamera)
                {
                    Camera.main.transform.position = dataDialog.CameraPosition;
                    Camera.main.transform.eulerAngles = dataDialog.CameraRotation;
                }
                canvasManager.NewDialog(dataDialog); 
                break;

            case Quest.ElementsTypes.Travel:
                Travel dataTravel = ActiveQuest.elements[questStage].data as Travel;
                Instantiate(Resources.Load("Quest Elements/Travel Route"), new Vector3(0, -1000, 0), Quaternion.identity).GetComponent<TravelRoute>().SetTravelRoute(dataTravel, this);
                if (dataTravel.objective != string.Empty) canvasManager.SetObjective(dataTravel.objective, true);

                break;

            case Quest.ElementsTypes.Fetch:
                Fetch dataFetch = ActiveQuest.elements[questStage].data as Fetch;

                if (dataFetch.objective != string.Empty) canvasManager.SetObjective(dataFetch.objective, true);

                break;

            case Quest.ElementsTypes.Slaughter:
                Slaughter dataSlaughter = ActiveQuest.elements[questStage].data as Slaughter;

                slaughterTargets = new();
                slaughterRemaining = new();
                foreach (Slaughter.Target target in dataSlaughter.targets)
                {
                    slaughterTargets.Add(target.creature.identity);
                    slaughterRemaining.Add(target.amount);
                }

                canvasManager.SetObjective(GetSlaughterObjective(), true);

                break;

            case Quest.ElementsTypes.Reward:
                Reward dataReward = ActiveQuest.elements[questStage].data as Reward;

                for(int i = 0; i<dataReward.reward.Length; i++)
                {
                    player.inventory.AddItem(dataReward.reward[i].item, dataReward.reward[i].amount, dataReward.notification);
                }
                QuestAdvance();

                break;
        }
    }

    public void QuestAdvance()
    {
        slaughterTargets = null;
        slaughterRemaining = null;
        Camera.main.GetComponent<CameraController>().Reset();

        if (!InQuest()) return;
        canvasManager.ClearObjective();

        questStage++;
        player.Freeze(false);
        if (questStage < ActiveQuest.elements.Length) QuestLogic();
        else QuestComplete();
    }

    public void QuestComplete()
    {
        canvasManager.NewAlert("QUEST COMPLETE", CanvasManager.AlertStyles.Quest);
        canvasManager.ClearObjective();

        if (ActiveQuest.mainQuest) blimp.AdvanceStage();

        if (questGiver != null) questGiver.QuestComplete();
        ActiveQuest = null;
        questGiver = null;
    }

    public void QuestCancel()
    {
        canvasManager.NewAlert($"{ActiveQuest.questName}\n QUEST CANCELLED", CanvasManager.AlertStyles.Quest);
        canvasManager.ClearObjective();

        ActiveQuest = null;
        questGiver = null;        
    }

    public void QuestStart(Quest newQuest, QuestGiver questGiver)
    {
        ActiveQuest = newQuest;
        this.questGiver = questGiver;
        questStage = 0;

        QuestLogic();
    }

    private string GetSlaughterObjective()
    {
        string objective = "Target List:";
        for (int i = 0; i < slaughterTargets.Count; i++)
            objective += $"\n{slaughterTargets[i]}\t {(slaughterRemaining[i] != 0 ? slaughterRemaining[i] + "x" : "CLEAR!")}";   

        return objective;
    }

    private void SlaughterProgress(string identity)
    {
        for (int i = 0; i < slaughterTargets.Count; i++)
        {
            if (slaughterTargets[i] == identity)
            {
                slaughterRemaining[i] = Mathf.Clamp(slaughterRemaining[i] - 1, 0, int.MaxValue);    
                canvasManager.SetObjective(GetSlaughterObjective(), false);

                break;
            }
            if (i == slaughterTargets.Count) return;
        }

        if (slaughterRemaining.Exists(x => x > 0)) return;

        canvasManager.NewAlert("Slaughter Complete!", CanvasManager.AlertStyles.Slaughter);
        QuestAdvance();
    }

    /// <summary>
    /// Remember to Add listeners to confirmationEvent and rejectionEvent!
    /// </summary>
    /// <param name="warning">can be empty if no warning needed</param>
    public void RequestConfirmation(string query, string warning)
    {
        player.Freeze(true);

        if (string.IsNullOrEmpty(warning)) canvasManager.NewConfirmation(query);
        else canvasManager.NewConfirmation(query, warning);
    }

    internal void Confirmation()
    {
        Debug.Log("Request Confirmed");
        player.Freeze(false);

        confirmationEvent.Invoke();
        confirmationEvent.RemoveAllListeners();
        rejectionEvent.RemoveAllListeners();
    }

    internal void Rejection()
    {
        Debug.Log("Request Rejected");
        player.Freeze(false);

        rejectionEvent.Invoke();
        rejectionEvent.RemoveAllListeners();
        confirmationEvent.RemoveAllListeners();
    }

    private void OnGUI()
    {
        if (!debug) return;

        GUIStyle style = new()
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold,
        };
        // Text
        GUI.Label(new Rect(10, 10, 100, 20), $"ms per frame: {System.Decimal.Round((decimal)(Time.deltaTime * 1000), 2)} ", style);
        // Buttons
        if (GUI.Button(new Rect(10, 40, 100, 20), "Reload")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (GUI.Button(new Rect(10, 70, 100, 20), "Unfreeze")) player.Freeze(false);
    }
}
