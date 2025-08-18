using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
        Image[] images = GetComponentsInChildren<Image>(true);
        if (images.Length > 1) icon = images[1];

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts.Length > 1)
        {
            textLevel = texts[0];
            textDesc = texts[1];
        }
    }
    void OnEnable()
    {
        if (playerCon != null && data != null)
        {
            level = playerCon.GetCurrentStatLevel(data.statusType);
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        if (data == null || textLevel == null || textDesc == null) return;

        if (icon != null) icon.sprite = data.statusIcon;
        textLevel.text = "Lv." + level;

        switch (data.statusType)
        {
            case StatusData.StatusType.HP:
            case StatusData.StatusType.Damage:
            case StatusData.StatusType.WeaponNumber:
                if (level < data.Status_Int.Length) textDesc.text = string.Format(data.statusDesc, data.Status_Int[level]);
                break;
            case StatusData.StatusType.BulletSize:
            case StatusData.StatusType.Speed:
            case StatusData.StatusType.Delay:
                if (level < data.Status_Flot.Length) textDesc.text = string.Format(data.statusDesc, data.Status_Flot[level]);
                break;
        }

        GetComponent<Button>().interactable = (level < data.maxLevel);
    }
    public void OnClick()
    {
        if (playerCon == null) return;

        switch (data.statusType)
        {
            case StatusData.StatusType.HP: 
                playerCon.UpgradePlayerHP();
                break;
            case StatusData.StatusType.Damage: 
                playerCon.UpgradePlayerDamage();
                break;
            case StatusData.StatusType.Speed: 
                playerCon.UpgradePlayerSpeed(); 
                break;
            case StatusData.StatusType.WeaponNumber: 
                playerCon.UpgradePlayerBulletLevel();
                break;
            case StatusData.StatusType.BulletSize: 
                playerCon.UpgradePlayerBulletSize(); 
                break;
            case StatusData.StatusType.Delay: 
                playerCon.UpgradePlayerDelay();
                break;
        }

        level++;

        if (GameManager.instance != null)
        {
            StartCoroutine(GameManager.instance.StatusSellectBarsBack());
        }
    }
}