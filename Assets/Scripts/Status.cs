using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Status : MonoBehaviour
{
    public StatusData data;
    public int level;
    public PlayerController playerCon;

    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject statusBars;

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
                textDesc.text = string.Format(data.statusDesc, data.Status_INT[level]);
                break;
            case StatusData.StatusType.BulletSize:
            case StatusData.StatusType.Speed:
                textDesc.text = string.Format(data.statusDesc, data.Status_FLOAT[level]);
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
            playerCon.bulletFireDelay -= (playerCon.bulletFireDelay * (data.Status_FLOAT[level] * 0.01f));
                break;
            case StatusData.StatusType.WeaponNumber:
                break;
            case StatusData.StatusType.BulletSize:
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

        GameManager.instance.Resume();
    }
}
