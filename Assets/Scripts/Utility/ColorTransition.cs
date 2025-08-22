using UnityEngine;
using UnityEngine.UI;

public class ColorTransition : MonoBehaviour
{
    public Image Target { get; private set; }
    public Color finalColor = Color.white;
    public AlphaTreatment alpha = AlphaTreatment.Both;
    public float duration = 1f;
    public bool ignoreTimeScale;
    public bool fadeOnStart = true;
    public enum AlphaTreatment { NoAlpha, OnlyAlpha, Both }

    void Start()
    {
        if (Target == null)
        {
            try { Target = GetComponent<Image>(); }
            catch { this.enabled = false; return; }
        }

        if (fadeOnStart) Fade();
    }

    public void Fade()
    {
        switch (alpha)
        {
            case AlphaTreatment.Both:
                Target.CrossFadeColor(finalColor, duration, ignoreTimeScale, true);
                break;

            case AlphaTreatment.NoAlpha:
                Target.CrossFadeColor(finalColor, duration, ignoreTimeScale, false);
                break;

            case AlphaTreatment.OnlyAlpha:
                Target.CrossFadeAlpha(finalColor.a, duration, ignoreTimeScale);
                break;
        }
    }
}
