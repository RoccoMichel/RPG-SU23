using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DialogManager : MonoBehaviour
{
    [Header("Attributes")]
    public bool richText; // custom system to change color and add effects to text
    public bool skippable; // can the play spam skip through this dialog?
    public Modes mode;
    public string title;
    public Sprite image;
    [TextArea] public List<string> dialogs;
    
    public enum Modes { Basic, Titled, TitledImage}

    [Header("References")]
    public TMP_Text dialogDisplay;
    public TMP_Text titleDisplay;
    public Image imageDisplay;
    private InputAction nextAction;
    public QuestGiver questGiver;
    [HideInInspector] public CanvasManager canvasManager;

    private int index;

    private void Start()
    {
        nextAction = InputSystem.actions.FindAction("Attack");
        SwitchModes(mode);
    }

    private void Update()
    {
        if (dialogs.Count <= 0) return; // early return because there is dialog

        dialogDisplay.text = dialogs[Mathf.Clamp(index, 0, dialogs.Count - 1)];

        if (nextAction.WasPressedThisFrame()) NextDialog();

        if (index >= dialogs.Count) FinishDialog();
    }

    public virtual void NewDialog(string[] contents)
    {
        dialogs = contents.ToList();
        SwitchModes(Modes.Basic);
    }

    public virtual void NewDialog(string[] contents, string title)
    {
        dialogs = contents.ToList();
        this.title = title;
        SwitchModes(Modes.Titled);
    }

    public virtual void NewDialog(string[] contents, string title, Sprite image)
    {
        index = 0;
        dialogs = contents.ToList();
        this.title = title;
        this.image = image;
        SwitchModes(Modes.TitledImage);
    }

    protected virtual void NextDialog()
    {
        index++;
    }

    protected virtual void FinishDialog()
    {
        questGiver.Finished();
        gameObject.SetActive(false);
    }

    public void SwitchModes(Modes mode)
    {
        this.mode = mode;

        switch (mode)
        {
            case Modes.Basic:
                dialogDisplay.gameObject.SetActive(true);
                titleDisplay.gameObject.SetActive(false);
                imageDisplay.gameObject.SetActive(false);
                break;

            case Modes.Titled:
                dialogDisplay.gameObject.SetActive(true);
                titleDisplay.gameObject.SetActive(true);
                imageDisplay.gameObject.SetActive(false);

                titleDisplay.text = title;
                break;

            case Modes.TitledImage:
                dialogDisplay.gameObject.SetActive(true);
                titleDisplay.gameObject.SetActive(true);
                imageDisplay.gameObject.SetActive(true);

                titleDisplay.text = title;
                imageDisplay.sprite = image;
                break;
        }
    }
}
