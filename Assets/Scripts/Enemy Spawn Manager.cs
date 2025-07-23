using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;
    public GameObject bossPrefab;

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

        if (enemySpawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawnManager: 스폰 포인트가 설정되지 않았습니다!");
            return;
        }

        Debug.Log($"스폰 포인트 개수: {enemySpawnPoints.Length}");
        StartStage(currentStage);
    }


    void Update()
    {
        if (!isPlayerAlive || currentStage > maxStage || enemySpawnedThisStage >= enemyToSpawnThisStage)
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
        List<GameObject> possibleEnemy = new List<GameObject>();
        if (currentStage >= 1 && currentStage <= 2) { possibleEnemy.Add(enemyPrefab1); }
        else if (currentStage >= 3 && currentStage <= 5) { possibleEnemy.Add(enemyPrefab1); possibleEnemy.Add(enemyPrefab2); }
        else { possibleEnemy.Add(enemyPrefab1); possibleEnemy.Add(enemyPrefab2); possibleEnemy.Add(enemyPrefab3); }

        if (possibleEnemy.Count == 0) return;

        GameObject prefabToSpawn = possibleEnemy[Random.Range(0, possibleEnemy.Count)];
        if (prefabToSpawn == null) return;

        Vector2 offset = Random.insideUnitCircle * 0.3f;
        Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, offset.y, 0);

        GameObject enemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawnManager = this;
        }

        enemySpawnedThisStage++;
        aliveEnemies++;
        Debug.Log($"일반 적 스폰! 스테이지: {currentStage}, 스폰된 수: {enemySpawnedThisStage}/{enemyToSpawnThisStage}");
    }


    void SpawnBoss()
    {
        if (enemySpawnPoints.Length > 4 && bossPrefab != null)
        {
            Vector2 offset = Random.insideUnitCircle * 0.3f;
            Vector3 spawnPos = enemySpawnPoints[4].position + new Vector3(offset.x, offset.y, 0); // 5번째 스폰 포인트는 보스 전용
            GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

            Enemy enemyScript = boss.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.spawnManager = this;
            }

            enemySpawnedThisStage++;
            aliveEnemies++;
            Debug.Log($"보스 스폰! 스테이지: {currentStage}");
        }
        else
        {
            Debug.LogError("보스 스폰 실패: 스폰포인트 부족(5개 이상 필요) 또는 보스 프리팹 없음");
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

    public void OnEnemyKilled()
    {
        aliveEnemies--;
        Debug.Log($"적 처치! 남은 적: {aliveEnemies}");

        if (enemySpawnedThisStage >= enemyToSpawnThisStage && aliveEnemies <= 0)
        {
            currentStage++;
            if (currentStage <= maxStage)
            {
                StartStage(currentStage);
            }
            else
            {
                Debug.Log("모든 스테이지 완료! 클리어!");
            }
        }
    }

    void StartStage(int stage)
    {
        enemySpawnedThisStage = 0;
        aliveEnemies = 0;
        enemyToSpawnThisStage = 3 + 4 * stage;
        bool spawnBoss = (stage % 5 == 0);

        if (spawnBoss)
            enemyToSpawnThisStage += 1;

        Debug.Log($"스테이지 {stage} 시작! 스폰할 적의 수: {enemyToSpawnThisStage}");
    }
}