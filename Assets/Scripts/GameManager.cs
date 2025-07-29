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

        if (playerObject != null)
            playerObject.SetActive(false);
        if (enemySpawnObject != null)
            enemySpawnObject.SetActive(false);
        if (hp_Gauge != null)
            hp_Gauge.SetActive(false);

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
        }
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

            if(playerObject != null)
                playerObject.SetActive(true);
            if(enemySpawnObject != null)
                enemySpawnObject.SetActive(true);
            if (hp_Gauge != null)
                hp_Gauge.SetActive(true);


            if (startButtonUI != null)
                startButtonUI.SetActive(false);


            if(backgroundObject != null)
            {
                Destroy(backgroundObject);
            }

            
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
            currentStageObject.transform.position = new Vector3(-2.8f, -0.5f, 0); // Reset position
        }
        else
        {
            Debug.LogWarning("Stage index out of bounds or prefab is null.");
        }
    }

    
    public void NextStage()
    {
        currentStage++;
        LoadStage(currentStage);
    }

}
