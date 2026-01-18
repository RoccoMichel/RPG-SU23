using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private Slider sliderDisplay;
    private PlayerBase player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
    }

    private void FixedUpdate()
    {
        healthDisplay.text = $"HP: {Mathf.CeilToInt(player.health)}";
        sliderDisplay.maxValue = player.maxHealth;
        sliderDisplay.value = player.health;
    }
}
