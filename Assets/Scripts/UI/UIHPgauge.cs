using UnityEngine;
using UnityEngine.UI;


public class UIHPgauge : MonoBehaviour
{
    public Slider slider;
    public void UpdateGauge(float amount)
    {
        this.slider.value = amount;
    }
}
