using UnityEngine;

public class QuestGiver : Entity
{
    [Header("Quest Giver Attributes")]
    public Quest Quest;
    private GameDirector director;

    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) StartQuest();
    }

    void StartQuest()
    {
        if (Quest == null)
        {
            Debug.LogWarning($"{identity} ({name}) has no quest to give!");
            return;
        }
        director.StartNewQuest(Quest);
    }
}
