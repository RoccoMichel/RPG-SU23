using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Statistics))]
public class GameDirector : MonoBehaviour
{
    public bool debug;
    public static GameDirector Instance;
    private InputAction debugAction;

    [Header("Quest Related")]
    public Quest ActiveQuest;
    private QuestGiver questGiver;
    [SerializeField] private int questStage;
    private List<string> elementTargets;
    private List<int> elementRemaining;
    private GameObject travelRoute;

    [Header("References")]
    public CanvasManager canvasManager;
    public Statistics statistics;
    public PlayerBase player;
    public Blimp blimp;

    [HideInInspector] public UnityEvent respawnPickUps;
    [HideInInspector] public UnityEvent respawnCreatures;
    [HideInInspector] public UnityEvent confirmationEvent;
    [HideInInspector] public UnityEvent rejectionEvent;

    public bool InQuest() { return ActiveQuest != null; }

    private void Start()
    {
        if (Instance == null ) Instance = this;

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

    public void ReportItem(Item item)
    {
        if (elementTargets != null) ProgressListObjective(item.itemName, Quest.ElementsTypes.Fetch);
    }

    public void ReportDeath(Creature deceased)
    {
        statistics.AddKill(deceased);
        if (elementTargets != null) ProgressListObjective(deceased.identity, Quest.ElementsTypes.Slaughter);
    }

    private void QuestLogic()
    {
        if (ActiveQuest == null) return;

        if (ActiveQuest.elements[questStage].data == null)
            Debug.LogError($"Quest Element Data is null! Quest: {ActiveQuest.questName}, Stage: {questStage}");

        switch (ActiveQuest.elements[questStage].type)
        {
            // DIALOG ELEMENT
            case Quest.ElementsTypes.Dialog:
                Dialog dataDialog = ActiveQuest.elements[questStage].data as Dialog;
                if (dataDialog.freezePlayer) player.Freeze(true);
                if (dataDialog.setCamera)
                {
                    Camera.main.transform.position = dataDialog.CameraPosition;
                    Camera.main.transform.eulerAngles = dataDialog.CameraRotation;
                }
                else if (dataDialog.CameraPosition != Vector3.zero || dataDialog.CameraRotation != Vector3.zero)
                    Debug.LogWarning("Dialog Camera Position/Rotation set but 'setCamera' is false!");
                canvasManager.NewDialog(dataDialog); 
                break;

            // TRAVEL ELEMENT
            case Quest.ElementsTypes.Travel:
                Travel dataTravel = ActiveQuest.elements[questStage].data as Travel;

                if (travelRoute != null) Destroy(travelRoute);
                travelRoute = Instantiate((GameObject)Resources.Load("Quest Elements/Travel Route"), new Vector3(0, -9001, 0), Quaternion.identity);
                travelRoute.GetComponent<TravelRoute>().SetTravelRoute(dataTravel, this);

                if (dataTravel.objective != string.Empty)
                {
                    canvasManager.SetObjective(dataTravel.objective, true);
                    StartCoroutine(TravelObjective(dataTravel.objective, questStage));
                }

                break;

            // FETCH ELEMENT
            case Quest.ElementsTypes.Fetch:
                Fetch dataFetch = ActiveQuest.elements[questStage].data as Fetch;

                elementTargets = new();
                elementRemaining = new();
                foreach (Fetch.Target target in dataFetch.FetchList)
                {
                    elementTargets.Add(target.type.itemName);
                    elementRemaining.Add(target.amount);
                }

                canvasManager.SetObjective(GetListRelatedObjective(Quest.ElementsTypes.Fetch), true);

                break;

            // SLAUGHTER ELEMENT
            case Quest.ElementsTypes.Slaughter:
                Slaughter dataSlaughter = ActiveQuest.elements[questStage].data as Slaughter;

                elementTargets = new();
                elementRemaining = new();
                foreach (Slaughter.Target target in dataSlaughter.targets)
                {
                    elementTargets.Add(target.creature.identity);
                    elementRemaining.Add(target.amount);
                }

                canvasManager.SetObjective(GetListRelatedObjective(Quest.ElementsTypes.Slaughter), true);

                break;

            // REWARD ELEMENT
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
    
    public Quest.ElementsTypes GetCurrentQuestElementType()
    {
        return ActiveQuest.elements[questStage].type;
    }

    public void QuestAdvance()
    {
        elementTargets = null;
        elementRemaining = null;
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

    public void QuestFail()
    {
        if (!InQuest()) return;

        canvasManager.NewAlert("QUEST FAILED", CanvasManager.AlertStyles.Quest);
        canvasManager.ClearObjective();

        if (questGiver != null) questGiver.QuestCancel();
        ActiveQuest = null;
        questGiver = null;
    }

    public void QuestStart(Quest newQuest, QuestGiver questGiver)
    {
        ActiveQuest = newQuest;
        this.questGiver = questGiver;
        questStage = 0;

        if (travelRoute != null) Destroy(travelRoute);

        QuestLogic();
    }

    private string GetListRelatedObjective(Quest.ElementsTypes type)
    {
        string objective;

        switch (type)
        {
            case Quest.ElementsTypes.Fetch:
                objective = "Collect the following:";
                break;
            case Quest.ElementsTypes.Slaughter:
                objective = "Slaughter the following:";
                break;
            default:
                Debug.LogError("GetListRelatedObjective called with non list-related type!");
                return string.Empty;
        }

        for (int i = 0; i < elementTargets.Count; i++)
            objective += $"\n{elementTargets[i]}\t {(elementRemaining[i] != 0 ? elementRemaining[i] + "x" : "- CLEAR -")}";   

        return objective;
    }

    private void ProgressListObjective(string identity, Quest.ElementsTypes type)
    {
        for (int i = 0; i < elementTargets.Count; i++)
        {
            if (elementTargets[i] == identity)
            {
                elementRemaining[i] = Mathf.Clamp(elementRemaining[i] - 1, 0, int.MaxValue);    
                canvasManager.SetObjective(GetListRelatedObjective(type), false);

                break;
            }
            if (i == elementTargets.Count) return;
        }

        if (elementRemaining.Exists(x => x > 0)) return;

        if (type == Quest.ElementsTypes.Fetch)
            canvasManager.NewAlert("Fetch Complete!", CanvasManager.AlertStyles.QuestElement);
        else if (type == Quest.ElementsTypes.Slaughter)
            canvasManager.NewAlert("Slaughter Complete!", CanvasManager.AlertStyles.QuestElement);
        else
            Debug.LogError("ProgressListObjective called with non list-related type!");

        QuestAdvance();
    }

    private System.Collections.IEnumerator TravelObjective(string objective, int objectiveStage)
    {
        yield return new WaitForEndOfFrame();

        int distance;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 finish = GameObject.FindGameObjectWithTag("Finish").transform.position;

        while (objectiveStage == questStage)
        {
            distance = Mathf.RoundToInt(Vector3.Distance(player.position, finish));
            canvasManager.SetObjective(objective + $"\n({distance}m)", false);
            yield return new WaitForEndOfFrame();
        }

        yield break;
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
        player.Freeze(false);

        confirmationEvent.Invoke();
        confirmationEvent.RemoveAllListeners();
        rejectionEvent.RemoveAllListeners();
    }

    internal void Rejection()
    {
        player.Freeze(false);

        rejectionEvent.Invoke();
        rejectionEvent.RemoveAllListeners();
        confirmationEvent.RemoveAllListeners();
    }

    public void RespawnPickUps()
    {
        respawnPickUps.Invoke();
    }

    public void RespawnCreatures()
    {
        respawnCreatures.Invoke();
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
        if (GUI.Button(new Rect(10, 100, 100, 20), "Quit")) Application.Quit();
        if (GUI.Button(new Rect(10, 130, 100, 20), "Quest Advance")) QuestAdvance();
        if (GUI.Button(new Rect(10, 160, 100, 20), "Quest Complete")) QuestComplete();
    }
}
