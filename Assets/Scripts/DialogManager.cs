using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DialogManager : MonoBehaviour
{
    [Header("Attributes")]
    private Modes mode;
    private string title;
    private Sprite image;
    [TextArea] public List<string> lines;

    public enum Modes { Basic, Titled, Image, TitledImage }

    [Header("References")]
    public TMP_Text dialogDisplay;
    public TMP_Text titleDisplay;
    public Image imageDisplay;
    private Dialog data;
    private InputAction interactAction;
    [HideInInspector] public CanvasManager canvasManager;

    private int lineIndex;
    private int conversationIndex;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");

        SwitchModes(mode);
    }

    private void Update()
    {
        if (lines.Count <= 0) return; // early return because there is dialog

        dialogDisplay.text = lines[Mathf.Clamp(lineIndex, 0, lines.Count - 1)];

        if (interactAction.WasPressedThisFrame()) NextLine();

        if (lineIndex >= lines.Count) FinishConversation();
    }

    public virtual void NewMessage(string[] contents)
    {
        lineIndex = 0;
        lines = contents.ToList();
        SwitchModes(Modes.Basic);
    }

    public virtual void NewMessage(string[] contents, string title)
    {
        lineIndex = 0;
        lines = contents.ToList();
        this.title = title;
        SwitchModes(Modes.Titled);
    }

    public virtual void NewMessage(string[] contents, Sprite image)
    {
        lineIndex = 0;
        lines = contents.ToList();
        this.image = image;
        SwitchModes(Modes.Image);
    }

    public virtual void NewMessage(string[] contents, string title, Sprite image)
    {
        lineIndex = 0;
        lines = contents.ToList();
        this.title = title;
        this.image = image;
        SwitchModes(Modes.TitledImage);
    }

    public virtual void NewDialog(Dialog data)
    {
        conversationIndex = -1;
        this.data = data;

        NextConversation();
    }

    protected virtual void NextLine()
    {
        lineIndex++;

        if (lineIndex >= lines.Count)
        {
            if (data != null) NextConversation();
            else FinishConversation();
        }
    }

    protected virtual void NextConversation()
    {
        lineIndex = 0;
        conversationIndex++;

        if (conversationIndex >= data.conversation.Length)
        {
            FinishConversation();
            return;
        }
        Dialog.Conversation currentConversation = data.conversation[conversationIndex];

        // Call correct Message format based on what is assigned in data
        if (currentConversation.speakerPortrait != null && currentConversation.title != string.Empty)
            NewMessage(currentConversation.lines.ToArray(), currentConversation.title, currentConversation.speakerPortrait);

        else if (currentConversation.title != string.Empty)
            NewMessage(currentConversation.lines.ToArray(), currentConversation.title);

        else if (currentConversation.speakerPortrait != null)
            NewMessage(currentConversation.lines.ToArray(), currentConversation.speakerPortrait);

        else
            NewMessage(currentConversation.lines.ToArray());
    }

    protected virtual void FinishConversation()
    {
        data = null;
        PlayerBase player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
        GameDirector director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        if (director.InQuest() && director.GetCurrentQuestElementType() == Quest.ElementsTypes.Dialog) director.QuestAdvance();
                
        player.Freeze(false);
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

            case Modes.Image:

                dialogDisplay.gameObject.SetActive(true);
                titleDisplay.gameObject.SetActive(false);
                imageDisplay.gameObject.SetActive(true);

                imageDisplay.sprite = image;
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