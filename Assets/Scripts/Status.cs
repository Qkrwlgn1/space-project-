using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Status : MonoBehaviour
{
    public StatusData data;
    public int level;
    public PlayerController playerCon;

    Image icon;
    TextMeshProUGUI textLevel;
    TextMeshProUGUI textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.statusIcon;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textLevel = texts[0];
        textDesc = texts[1];
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.statusType)
        {
            case StatusData.StatusType.HP:
            case StatusData.StatusType.Damage:
            case StatusData.StatusType.WeaponNumber:
                textDesc.text = string.Format(data.statusDesc, data.Status_Int[level]);
                break;
            case StatusData.StatusType.BulletSize:
            case StatusData.StatusType.Speed:
                textDesc.text = string.Format(data.statusDesc, data.Status_Flot[level]);
                break;
        }
        
    }
    public void OnClick()
    {
        switch (data.statusType)
        {
            case StatusData.StatusType.HP:
                break;
            case StatusData.StatusType.Damage:
                break;
            case StatusData.StatusType.Speed:
            playerCon.bulletFireDelay -= (playerCon.bulletFireDelay * (data.Status_Flot[level] * 0.01f));
                break;
            case StatusData.StatusType.WeaponNumber:
                break;
            case StatusData.StatusType.BulletSize:
                break;
        }

        level++;
        StartCoroutine(GameManager.instance.StatusSellectBarsBack());


        if (level == 5)
        {
            GetComponent<Button>().interactable = false;
        }

    }

    
}
