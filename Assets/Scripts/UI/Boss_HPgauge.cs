using UnityEngine;
using UnityEngine.UI;


public class Boss_HPgauge : MonoBehaviour
{
    public Slider slider;
    public static bool isBossAlive = false;
    private bool BossLiveLogic;
    [SerializeField] private Boss _boss;
    [SerializeField] private GameObject bossHPSlider;

    void Update()
    {
        // State Caching
        if (isBossAlive != BossLiveLogic)
        {
            bossHPSlider.SetActive(isBossAlive);
            BossLiveLogic = isBossAlive;
        }

        if (isBossAlive == true)
        {
            UpdateGauge(_boss.enemyCurrentHealth);
        }
    }

    public void UpdateGauge(float amount)
    {
        this.slider.value = amount;
    }
}
