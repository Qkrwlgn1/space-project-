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
        if (enemySpawnPoints == null || enemySpawnPoints.Length < 9)
        {
            Debug.LogError("EnemySpawnManager: 9개의 스폰 포인트가 모두 설정되지 않았습니다!");
            return;
        }

        StartStage(currentStage);
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
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

    // ### 수정: 새로운 스폰 로직의 메인 함수 ###
    void SpawnBatch()
    {
        // 1. 현재 스테이지에서 스폰 가능한 포인트 목록을 가져옵니다.
        List<Transform> validPoints = GetValidSpawnPointsForStage(currentStage);

        // 2. 그중에서 현재 비어있는 포인트만 추려냅니다.
        List<Transform> availablePoints = FindUnoccupiedPoints(validPoints);
        if (availablePoints.Count == 0) return;

        // 3. 스폰할 적의 수를 정합니다.
        int enemiesToSpawnInBatch = Random.Range(1, 4);
        enemiesToSpawnInBatch = Mathf.Min(enemiesToSpawnInBatch, availablePoints.Count);

        for (int i = 0; i < enemiesToSpawnInBatch; i++)
        {
            if (aliveEnemies >= maxAliveEnemies || enemySpawnedThisStage >= enemyToSpawnThisStage) break;

            // 4. 비어있는 스폰 포인트 중 하나를 무작위로 선택합니다.
            int randIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randIndex];

            // 5. 해당 스폰 포인트의 인덱스를 찾아 그에 맞는 적을 스폰합니다.
            int pointIndex = System.Array.IndexOf(enemySpawnPoints, spawnPoint);
            string tagToSpawn = GetEnemyTagForPointIndex(pointIndex);

            SpawnEnemy(tagToSpawn, spawnPoint);

            availablePoints.RemoveAt(randIndex);
        }
    }

    // ### 추가: 스테이지별로 유효한 스폰 포인트 목록을 반환하는 함수 ###
    private List<Transform> GetValidSpawnPointsForStage(int stage)
    {
        List<Transform> validPoints = new List<Transform>();

        // Enemy1 스폰 포인트 (3, 4, 6, 7번 -> 인덱스 2, 3, 5, 6)
        validPoints.Add(enemySpawnPoints[2]);
        validPoints.Add(enemySpawnPoints[3]);
        validPoints.Add(enemySpawnPoints[5]);
        validPoints.Add(enemySpawnPoints[6]);

        // 3스테이지부터 Enemy2 스폰 포인트 추가 (2, 8번 -> 인덱스 1, 7)
        if (stage >= 3)
        {
            validPoints.Add(enemySpawnPoints[1]);
            validPoints.Add(enemySpawnPoints[7]);
        }

        // 6스테이지부터 Enemy3 스폰 포인트 추가 (1, 9번 -> 인덱스 0, 8)
        if (stage >= 6)
        {
            validPoints.Add(enemySpawnPoints[0]);
            validPoints.Add(enemySpawnPoints[8]);
        }

        return validPoints;
    }

    // ### 추가: 스폰 포인트 인덱스에 따라 적 태그를 반환하는 함수 ###
    private string GetEnemyTagForPointIndex(int index)
    {
        switch (index)
        {
            case 1: // 2번 스폰 포인트
            case 7: // 8번 스폰 포인트
                return enemyTag2;
            case 0: // 1번 스폰 포인트
            case 8: // 9번 스폰 포인트
                return enemyTag3;
            default: // 3, 4, 5(보스), 6, 7번 스폰 포인트
                return enemyTag1;
        }
    }

    // ### 추가: 주어진 포인트 목록 중 비어있는 곳만 반환하는 함수 ###
    private List<Transform> FindUnoccupiedPoints(List<Transform> pointsToCheck)
    {
        List<Transform> availablePoints = new List<Transform>(pointsToCheck);
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in activeEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            Transform closestPoint = null;
            float minDistance = 1.5f; // 점유 판단 반경을 조금 늘림

            foreach (Transform point in pointsToCheck) // 전체 유효 포인트와 비교
            {
                float distance = Vector3.Distance(enemy.transform.position, point.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }
            if (closestPoint != null)
            {
                availablePoints.Remove(closestPoint);
            }
        }
        return availablePoints;
    }

    // ### 추가: 적을 스폰하는 단순 헬퍼 함수 ###
    void SpawnEnemy(string tag, Transform spawnPoint)
    {
        Vector2 offset = Random.insideUnitCircle * 0.3f;
        Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, offset.y, 0);

        GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(tag, spawnPosition, Quaternion.identity);
        if (enemyObj == null) return;

        Enemy enemyScript = enemyObj.GetComponent<Enemy>();
        if (enemyScript != null) enemyScript.spawnManager = this;

        enemySpawnedThisStage++;
        aliveEnemies++;
    }

    // ### 기존 함수들은 거의 그대로 유지 ###
    #region 기존 함수들 (변경 없음)
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
    void SpawnBoss()
    {
        // 보스는 5번 스폰 포인트(인덱스 4)에서만 생성
        if (enemySpawnPoints.Length > 4)
        {
            SpawnEnemy(bossTag, enemySpawnPoints[4]);
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
    #endregion
}