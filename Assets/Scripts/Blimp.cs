using UnityEngine;

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
        currentStage = Mathf.Clamp(currentStage + 1, 0, stages.Length - 1);
        GetComponent<MeshFilter>().mesh = stages[currentStage];

        if (currentStage == stages.Length - 1) complete = true;
    }
}
