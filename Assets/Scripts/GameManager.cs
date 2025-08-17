using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameStarted;
    public bool isLive;
    public int stageloadIndex;

    [Header("Game Objects")]
    public GameObject playerObject;
    public GameObject enemySpawnObject;
    public GameObject startButtonUI;
    public GameObject hp_Gauge;


    [Header("Stage Management")]
    public GameObject[] backgroundObject;
    private GameObject currentStageObject;

    [Header("Enternal Scripts")]
    public EnemySpawnManager enemySpawn;
    public PlayerController playerCon;
    public SettingMenu settingMenu;
    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject statusBars;

    [Header("PoolsTag")]
    public string StageTag_1 = "StageBG_1";
    public string StageTag_2 = "StageBG_2";
    public string StageTag_3 = "StageBG_3";

    void Awake()
    {
        instance = this;
        isGameStarted = false;
    }

    void Start()
    {
        AudioManagerScript.Instance.PlayBgm(0);
        settingMenu._menu[4].SetActive(false);
        playerObject.SetActive(false);
        enemySpawnObject.SetActive(false);
        hp_Gauge.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
            enabled = false;
        }
    }

    public void StartGame()
    {
        if (!isGameStarted)
        {
            settingMenu.scoreText.gameObject.SetActive(true);
            isGameStarted = true;
            playerObject.SetActive(true);
            enemySpawnObject.SetActive(true);
            hp_Gauge.SetActive(true);
            startButtonUI.SetActive(false);
            foreach (GameObject obj in backgroundObject)
            {
                obj.SetActive(false);
            }


            LoadStage(enemySpawn.currentStage);
        }
    }

    public void LoadStage(int stageIndex)
    {
        if (currentStageObject != null)
        {
            currentStageObject.SetActive(false);
        }

        if (stageIndex > 0 && stageIndex < 4)
        {
            currentStageObject = ObjectPooler.Instance.SpawnFromPool(StageTag_1, new Vector3(-2.8f, -0.5f, 0), Quaternion.identity);
            AudioManagerScript.Instance.PlayBgm(1);
            
        }
        else if (stageIndex >= 4 && stageIndex < 7)
        {
            currentStageObject = ObjectPooler.Instance.SpawnFromPool(StageTag_2, new Vector3(-2.8f, -0.5f, 0), Quaternion.identity);
            if (stageIndex == 5)
            {
                AudioManagerScript.Instance.PlayBgm(6);
            }
            else
            {
                AudioManagerScript.Instance.PlayBgm(2);
            }
        }
        else if (stageIndex >= 7 && stageIndex < 11)
        {
            currentStageObject = ObjectPooler.Instance.SpawnFromPool(StageTag_3, new Vector3(-2.8f, -0.5f, 0), Quaternion.identity);
            if (stageIndex == 10)
            {
                AudioManagerScript.Instance.PlayBgm(6);
            }
            else
            {
                AudioManagerScript.Instance.PlayBgm(3);
            }
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

    public IEnumerator ItemSellectBars()
    {
        Stop();

        itemBack.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        playerCon.Next();
        statusBars.SetActive(true);
        
    }

    public IEnumerator StatusSellectBarsBack()
    {
        itemBack.SetActive(false);
        statusBars.SetActive(false);
        Resume();
        yield return new WaitForSeconds(0f);
    }
}
