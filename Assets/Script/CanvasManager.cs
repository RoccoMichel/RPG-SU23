using Unity.VisualScripting;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [HideInInspector] public DialogManager dialog;

    public void NewDialog(string[] contents)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents);
    }
    public void NewDialog(string[] contents, string title)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents, title);
    }
    public void NewDialog(string[] contents, string title, Sprite image)
    {
        if (dialog == null) dialog = InstantiateDialog();
        if (!dialog.gameObject.activeSelf) dialog.gameObject.SetActive(true);
        dialog.NewDialog(contents, title, image);
    }
    public void AssignQuestGiver(QuestGiver questGiver)
    {
        dialog.questGiver = questGiver;
    }

    private DialogManager InstantiateDialog()
    {
        DialogManager newDialog = Instantiate(Resources.Load("UI/DialogSystem"), gameObject.transform).GetComponent<DialogManager>();
        newDialog.canvasManager = this;
        return newDialog;
    }
}
