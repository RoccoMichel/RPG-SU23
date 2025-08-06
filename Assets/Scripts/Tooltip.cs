using TMPro;
using UnityEngine;

public class Tooltip : CursorBoundUI
{
    [SerializeField] private TMP_Text titleDisplay;
    [SerializeField] private TMP_Text descriptionDisplay;

    public void GenerateTooltip(string title, string description)
    {
        titleDisplay.text = title;
        descriptionDisplay.text = description;
    }
}
