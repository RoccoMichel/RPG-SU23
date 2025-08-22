using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public bool debug;
    [Header("Quest Related")]
    public Quest ActiveQuest;
    private QuestGiver questGiver;
    [SerializeField] private int questStage;

    [Header("References")]
    public CanvasManager canvasManager;
    public Statistics statistics;
    public PlayerBase player;

    public bool InQuest() { return ActiveQuest != null; }

    private void Start()
    {
        try { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); }
        catch { Debug.LogWarning("No Player active in Scene!"); }

        if (canvasManager == null)
        {
            try { canvasManager = FindFirstObjectByType<CanvasManager>().GetComponent<CanvasManager>(); }
            catch { Debug.LogError("No CanvasManager active in Scene!"); Debug.Break(); }
        }

        statistics = GetComponent<Statistics>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) debug = !debug;
    }

    private void QuestLogic()
    {
        if (ActiveQuest == null) return;

        switch(ActiveQuest.elements[questStage].type)
        {
            case Quest.ElementsTypes.Dialog:
                Dialog dataDialog = ActiveQuest.elements[questStage].data as Dialog;
                if (dataDialog.freezePlayer) player.Freeze(true);
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

                if (dataSlaughter.objective != string.Empty) canvasManager.SetObjective(dataSlaughter.objective, true);

                break;
        }
    }

    public void QuestAdvance()
    {
        if (!InQuest()) return;

        questStage++;
        player.Freeze(false);
        if (questStage < ActiveQuest.elements.Count) QuestLogic();
        else QuestComplete();
    }

    public void QuestComplete()
    {
        canvasManager.NewAlert("QUEST COMPLETE");
        if (questGiver != null) questGiver.QuestComplete();
        ActiveQuest = null;
        questGiver = null;
    }

    public void QuestCancel()
    {
        canvasManager.NewAlert($"{ActiveQuest.questName}\n quest cancelled");

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
