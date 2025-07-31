using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Status : MonoBehaviour
{
    public StatusData data;
    public int level;
    public PlayerController playerCon;
    public BulletController bulletCon;

    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject statusBars;

    Image icon;
    TextMeshProUGUI textLevel;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.statusIcon;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textLevel = texts[0];
    }

    void LateUpdate()
    {
        textLevel.text = "Lv." + (level + 1);
    }

    public void OnClick()
    {
        switch (data.statusType)
        {
            case StatusData.StatusType.HP:
                playerCon.playerCurrentHealth += data.HP_Steps[level];
                break;
            case StatusData.StatusType.Damage:
                bulletCon.playerBulletDamage += data.Damage_Steps[level];
                break;
            case StatusData.StatusType.Speed:
                playerCon.bulletFireDelay += data.Speed_Steps[level];
                break;
            case StatusData.StatusType.WeaponNumber:
                break;
        }

        level++;
        StartCoroutine(ItemSellectBarsBack());


        if (level == 5)
        {
            GetComponent<Button>().interactable = false;
        }

    }
    
    IEnumerator ItemSellectBarsBack()
    {
        itemBack.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        statusBars.SetActive(false);  
    }
}
