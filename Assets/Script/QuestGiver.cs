using UnityEngine;

public class QuestGiver : Entity
{
    [HideInInspector] public bool questActive;
    public CanvasManager canvasManager;
    public Transform cameraPosition;
    [TextArea] public string[] dialog;
    private CameraController cameraController;

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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().freeze = true;
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.locked = true;
        cameraController.gameObject.transform.SetPositionAndRotation(cameraPosition.position, cameraPosition.rotation);
        canvasManager.NewDialog(dialog, identity);
        canvasManager.AssignQuestGiver(this);
    }

    public void Finished()
    {
        questActive = false;
        cameraController.locked = false;
        cameraController.ResetRotation();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().freeze = false;
    }
}
