using UnityEngine;
using UnityEngine.SceneManagement;

public class Blimp : MonoBehaviour
{
    public bool complete;
    public int currentStage = 0;
    public Mesh[] stages;

    private void Start()
    {
        if (stages.Length == 0)
        {
            Debug.LogWarning("No stages assigned to Blimp!");
            return;
        }
        GetComponent<MeshFilter>().mesh = stages[currentStage];
    }

    public void AdvanceStage()
    {
        if (complete) return;

        currentStage = Mathf.Clamp(currentStage + 1, 0, stages.Length - 1);
        GetComponent<MeshFilter>().mesh = stages[currentStage];

        if (currentStage == stages.Length - 1)
        {
            complete = true;
        }
    }

    public void Interact()
    {
        if (complete)
        {
            GameDirector.Instance.RequestConfirmation("Enter the Blimp?", "this will end the game");
            GameDirector.Instance.confirmationEvent.AddListener(LeaveConfirm);
        }
        else
        {
            GameDirector.Instance.player.Freeze(true);
            int percentage = Mathf.RoundToInt((float)currentStage / (stages.Length - 1) * 100);
            GameDirector.Instance.canvasManager.NewMessage(new string[] { $"The blimp is {percentage}% done.", "A couple more quests should do the trick!" }, "Daniel");
        }        
    }

    private void LeaveConfirm()
    {
        SceneManager.LoadScene(0);
    }
}
