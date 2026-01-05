using TMPro;
using UnityEngine;

public class ConfirmationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text queryDisplay;
    [SerializeField] private TMP_Text warningDisplay;


    public void SetValues(string query)
    {
        queryDisplay.text = query;
        warningDisplay.gameObject.SetActive(false);
    }

    public void SetValues(string query, string warning)
    {
        queryDisplay.text = query;
        warningDisplay.text = warning;
    }

    public void Confirm()
    {
        GameDirector.instance.Confirmation();
        Destroy(gameObject);
    }

    public void Reject()
    {
        GameDirector.instance.Rejection();
        Destroy(gameObject);
    }
}
