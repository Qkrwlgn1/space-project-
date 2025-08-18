using UnityEngine;
using UnityEngine.UI;

public class UIHPgauge : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
    }
    public void UpdateGauge(float amount)
    {
        if (slider != null)
        {
            slider.value = amount;
        }
    }
}