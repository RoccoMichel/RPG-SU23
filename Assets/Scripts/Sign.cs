using TMPro;
using UnityEngine;

public class Sign : MonoBehaviour
{
    private GameDirector director;
    [Header("Can be left empty if sign has TMP Text in child")]
    public string text = string.Empty;

    private void Start()
    {
        director = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
        if (text == string.Empty)
        {
            try { text = gameObject.GetComponentInChildren<TMP_Text>().text; }
            finally { }
        }
    }

    public void Read()
    {
        director.canvasManager.NewMessage(new string[] { "It reads: ", text == string.Empty ? "*you couldn't decipher it*" : text });
        director.player.Freeze(true);
    }
}
