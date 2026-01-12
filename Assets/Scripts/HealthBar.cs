using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private string title;
    private float value;
    private float min;
    private float max;
    [Header("References")]
    [SerializeField] private TMP_Text titleDisplay;
    [SerializeField] private Slider slider;

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
        slider.value = value;
    }

    public void Set(float minimum, float maximum, float value)
    {
        min = minimum; max = maximum;
        this.value = Mathf.Clamp(value, min, max);

        if (slider == null)
        {
            try { slider = GetComponentInChildren<Slider>(); }
            catch { Debug.LogError("HealthBar Prefab is missing 'Slider' Component in Children!"); return; }
        }
        slider.maxValue = max;
        slider.minValue = min;
        slider.value = value;
    }
    public void Set(float minimum, float maximum, float value, int level, string identity)
    {
        Set(minimum, maximum, value);
        title = $"lv. {level} {identity}";

        if (titleDisplay == null)
        {
            try { titleDisplay = GetComponentInChildren<TMP_Text>(); }
            catch { Debug.LogError("HealthBar Prefab is missing 'TMP_Text' Component in Children!"); return; }
        }
        titleDisplay.text = title;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
