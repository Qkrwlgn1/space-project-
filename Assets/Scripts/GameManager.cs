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
    public GameObject[] stagePrefabs;
    private GameObject currentStageObject;

    [Header("External Scripts")]
    public EnemySpawnManager enemySpawn;
    public PlayerController playerCon;
    public SettingMenu settingMenu;
    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject statusBars;

    void Awake()
    {
        instance = this;
        isGameStarted = false;
        isLive = true;
        playerObject.SetActive(false);
        enemySpawnObject.SetActive(false);
        hp_Gauge.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isGameStarted)
        {
            StartGame();
        }
    }

    public IEnumerator ItemSellectBars()
    {
        itemBack.SetActive(true);
        yield return new WaitForSecondsRealtime(1f); // Time.timeScale 영향 안 받음

        if (playerCon != null) playerCon.Next(); // PlayerController의 Next() 호출

        statusBars.SetActive(true);
        Stop();
    }

    public IEnumerator StatusSellectBarsBack()
    {
        itemBack.SetActive(false);
        statusBars.SetActive(false);
        Resume();
        yield return null;
    }

    #region 기존 함수들 (수정 없음)
    public void StartGame()
    {
        if (!isGameStarted)
        {
            if (settingMenu != null && settingMenu.scoreText != null)
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
            stageloadIndex = enemySpawn.currentStage;
            LoadStage(stageloadIndex);
        }
    }
    private void LoadStage(int stageIndex)
    {
        if (currentStageObject != null)
        {
            Destroy(currentStageObject);
        }
        if (stageIndex > 0 && stageIndex <= stagePrefabs.Length)
        {
            currentStageObject = Instantiate(stagePrefabs[stageIndex - 1]);
            currentStageObject.transform.position = new Vector3(-2.8f, -0.5f, 0);
        }
    }
    public void NextStage()
    {
        stageloadIndex++;
        LoadStage(stageloadIndex);
    }
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 1;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
    #endregion
}