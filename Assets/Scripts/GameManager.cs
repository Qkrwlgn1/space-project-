using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool isGameStarted;

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

    void Awake()
    {
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

}
