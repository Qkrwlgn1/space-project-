using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameStarted;
    public bool isLive;

    [Header("Game Objects")]
    public GameObject playerObject;
    public GameObject enemySpawnObject;
    public GameObject startButtonUI;
    public GameObject hp_Gauge;


    [Header("Stage Management")]
    public int currentStage = 1;
    public GameObject backgroundObject;
    public GameObject[] stagePrefabs;
    private GameObject currentStageObject;

    [Header("Enternal Scripts")]
    public EnemySpawnManager enemySpawn;

    public PlayerController playerCon;
    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject statusBars;

    void Awake()
    {
        instance = this;
        isGameStarted = false;
        playerObject.SetActive(false);
        enemySpawnObject.SetActive(false);
        hp_Gauge.SetActive(false);
        backgroundObject.SetActive(true);
    }

    void Update()
    {
        if (Input.anyKey)
        {
            StartGame();
            enabled = false;
        }
    }


    public void StartGame()
    {
        if (!isGameStarted)
        {
            isGameStarted = true;
            playerObject.SetActive(true);
            enemySpawnObject.SetActive(true);
            hp_Gauge.SetActive(true);
            startButtonUI.SetActive(false);
            Destroy(backgroundObject);


            int stageloadIndex = 1;
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
        currentStage++;
        LoadStage(currentStage);
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

        itemBack.SetActive(true);

        yield return new WaitForSeconds(1f);

        playerCon.Next();
        statusBars.SetActive(true);

        Stop();
    }

    public IEnumerator StatusSellectBarsBack()
    {
        itemBack.SetActive(false);
        statusBars.SetActive(false);
        Resume();
        yield return new WaitForSeconds(0f);
    }
}
