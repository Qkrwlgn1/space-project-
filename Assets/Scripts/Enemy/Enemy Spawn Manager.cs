using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    [Header("Pool Tags")]
    public string enemyTag1 = "Enemy1";
    public string enemyTag2 = "Enemy2";
    public string enemyTag3 = "Enemy3";
    public string bossTag = "Boss";
    public string guaranteedItemTag = "Item";

    [Header("Spawning Info")]
    public Transform[] enemySpawnPoints;
    public float enemySpawnDelay;
    private float currentEnemySpawnDelay;
    public int currentStage = 1;
    public int maxStage = 10;
    public int maxAliveEnemies = 10;

    private int aliveEnemies = 0;
    private int enemySpawnedThisStage = 0;
    private int enemyToSpawnThisStage = 0;

    public bool isPlayerAlive = true;

    [Header("Stage Settings")]
    public float stageClearDelay = 3.0f;

    private bool hasItemDroppedThisStage = false;
    private bool isStageTransitioning = false;

    public bool HasItemDroppedThisStage => hasItemDroppedThisStage;


    void Start()
    {

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            int childCount = transform.childCount;
            enemySpawnPoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                enemySpawnPoints[i] = transform.GetChild(i);
            }
        }
        
        StartStage(currentStage);
    }


    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isPlayerAlive || currentStage > maxStage || enemySpawnedThisStage >= enemyToSpawnThisStage || isStageTransitioning)
            return;

        currentEnemySpawnDelay += Time.deltaTime;

        if (currentEnemySpawnDelay > enemySpawnDelay)
        {
            bool isBossTurn = (currentStage % 5 == 0) && (enemySpawnedThisStage == enemyToSpawnThisStage - 1);
            if (isBossTurn)
            {
                SpawnBoss();
            }
            else
            {
                SpawnBatch();
            }
            currentEnemySpawnDelay = 0f;
            enemySpawnDelay = Random.Range(1.5f, 3f);
        }
    }


    public void OnEnemyKilled(Vector3 deadEnemyPosition)
    {
        aliveEnemies--;

        if (!isStageTransitioning && enemySpawnedThisStage >= enemyToSpawnThisStage && aliveEnemies <= 0)
        {
            isStageTransitioning = true;

            if (!hasItemDroppedThisStage)
            {
                ObjectPooler.Instance.SpawnFromPool(guaranteedItemTag, deadEnemyPosition, Quaternion.identity);
            }
            StartCoroutine(NextStageRoutine());
            
        }
    }


    void StartStage(int stage)
    {
        hasItemDroppedThisStage = false;
        isStageTransitioning = false;
        enemySpawnedThisStage = 0;
        aliveEnemies = 0;
        enemyToSpawnThisStage = 3 + 4 * stage;
        bool spawnBoss = (stage % 5 == 0);
        if (spawnBoss)
            enemyToSpawnThisStage += 1;
    }


    void SpawnBatch()
    {
        List<Transform> availablePoints = new List<Transform>(enemySpawnPoints);
        Shuffle(availablePoints);
        int enemiesToSpawnInBatch = Random.Range(1, 3);
        for (int i = 0; i < enemiesToSpawnInBatch; i++)
        {
            if (i >= availablePoints.Count || aliveEnemies >= maxAliveEnemies || enemySpawnedThisStage >= enemyToSpawnThisStage)
            {
                break;
            }
            Transform spawnPoint = availablePoints[i];
            SpawnRegularEnemy(spawnPoint);
        }
    }


    void SpawnRegularEnemy(Transform spawnPoint)
    {
        List<string> possibleEnemyTags = new List<string>();
        if (currentStage >= 1 && currentStage <= 2) { possibleEnemyTags.Add(enemyTag1); }
        else if (currentStage >= 3 && currentStage <= 5) { possibleEnemyTags.Add(enemyTag1); possibleEnemyTags.Add(enemyTag2); }
        else { possibleEnemyTags.Add(enemyTag1); possibleEnemyTags.Add(enemyTag2); possibleEnemyTags.Add(enemyTag3); }

        if (possibleEnemyTags.Count == 0) return;

        string tagToSpawn = possibleEnemyTags[Random.Range(0, possibleEnemyTags.Count)];

        Vector2 offset = Random.insideUnitCircle * 0.3f;
        Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, offset.y, 0);

        GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(tagToSpawn, spawnPosition, Quaternion.identity);
        if (enemyObj == null) return;

        Enemy enemyScript = enemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawnManager = this;
        }

        enemySpawnedThisStage++;
        aliveEnemies++;
    }


    void SpawnBoss()
    {
        if (enemySpawnPoints.Length > 4)
        {
            Vector2 offset = Random.insideUnitCircle * 0.3f;
            Vector3 spawnPos = enemySpawnPoints[4].position + new Vector3(offset.x, offset.y, 0);

            GameObject bossObj = ObjectPooler.Instance.SpawnFromPool(bossTag, spawnPos, Quaternion.identity);
            if (bossObj == null) return;

            Enemy enemyScript = bossObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.spawnManager = this;
            }

            enemySpawnedThisStage++;
            aliveEnemies++;
        }
    }


    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }


    public void NotifyItemDropped()
    {
        hasItemDroppedThisStage = true;
    }


    private IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(stageClearDelay);
        currentStage++;
        if (currentStage <= maxStage)
        {
            StartStage(currentStage);
        }
    }
}