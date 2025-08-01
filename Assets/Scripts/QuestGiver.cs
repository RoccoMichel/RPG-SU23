using UnityEngine;

public class QuestGiver : Entity
{
    [HideInInspector] public bool questActive;
    public CanvasManager canvasManager;
    public Transform cameraPosition;
    [TextArea] public string[] dialog;
    private PlayerBase player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !questActive)
        {
            StartQuest();
        }
    }

    void StartQuest()
    {
        questActive = true;
        player.Freeze();
        player.cameraController.locked = true;
        player.cameraController.gameObject.transform.SetPositionAndRotation(cameraPosition.position, cameraPosition.rotation);
        canvasManager.NewDialog(dialog, identity);
        canvasManager.AssignQuestGiver(this);
    }

    public void Finished()
    {
        questActive = false;
        player.Unfreeze();
        player.cameraController.ResetRotation();
    }
}
