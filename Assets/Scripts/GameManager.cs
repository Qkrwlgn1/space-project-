using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameStarted;
    

    public GameObject playerObject;
    public GameObject enemySpawnObject;
    public GameObject startButtonUI;
    

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

        if(backgroundObject != null)
        {
            backgroundObject.SetActive(true);
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

            if(startButtonUI != null)
                startButtonUI.SetActive(false);


            if(backgroundObject != null)
            {
                Destroy(backgroundObject);
            }

            // 스테이지 로드
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

    // 이 함수는 나중에 스테이지 클리어 시 호출하면 됨
    public void NextStage()
    {
        currentStage++;
        LoadStage(currentStage);
    }

    void Update()
    {
        if(isGameStarted && Input.GetKeyDown(KeyCode.N))
        {
            NextStage();
        }
    }
}
