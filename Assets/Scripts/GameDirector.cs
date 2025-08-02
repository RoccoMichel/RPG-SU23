using Unity.VisualScripting;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [Header("Quest Related")]
    public Quest ActiveQuest { get; private set; }
    private int questStage;

    [Header("References")]
    public CanvasManager canvasManager;
    
    private void Start()
    {
        if (canvasManager == null)
        {
            try { canvasManager = FindFirstObjectByType<CanvasManager>().GetComponent<CanvasManager>(); }
            catch { Debug.LogError("No CanvasManager active in Scene!"); Debug.Break(); }
        }
    }

    private void QuestLogic()
    {
        if (ActiveQuest == null) return;

        switch(ActiveQuest.elements[questStage].type)
        {
            case Quest.ElementsTypes.Dialog:
                canvasManager.NewDialog(ActiveQuest.elements[questStage].data.GetComponent<Dialog>()); 
                break;

            case Quest.ElementsTypes.Travel:

                break;

            case Quest.ElementsTypes.Fetch:

                break;
        }
    }

    public void AdvanceQuest()
    {
        questStage++;
        if (questStage >= ActiveQuest.elements.Count) FinishQuest();
    }

    public void FinishQuest()
    {
        ActiveQuest = null;
    }

    public void StartNewQuest(Quest newQuest)
    {
        ActiveQuest = newQuest;
        questStage = 0;

        QuestLogic();
    }
}
