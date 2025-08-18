using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameManager gameManager;

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
            Debug.LogError("EnemySpawnManager: 9���� ���� ����Ʈ�� ��� �������� �ʾҽ��ϴ�!");
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
                Boss_HPgauge.isBossAlive = true;
                AudioManagerScript.Instance.PlayBgm(6);
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

    // ### ����: ���ο� ���� ������ ���� �Լ� ###
    void SpawnBatch()
    {
        // 1. ���� ������������ ���� ������ ����Ʈ ����� �����ɴϴ�.
        List<Transform> validPoints = GetValidSpawnPointsForStage(currentStage);

        // 2. ���߿��� ���� ����ִ� ����Ʈ�� �߷����ϴ�.
        List<Transform> availablePoints = FindUnoccupiedPoints(validPoints);
        if (availablePoints.Count == 0) return;

        // 3. ������ ���� ���� ���մϴ�.
        int enemiesToSpawnInBatch = Random.Range(1, 4);
        enemiesToSpawnInBatch = Mathf.Min(enemiesToSpawnInBatch, availablePoints.Count);

        for (int i = 0; i < enemiesToSpawnInBatch; i++)
        {
            if (aliveEnemies >= maxAliveEnemies || enemySpawnedThisStage >= enemyToSpawnThisStage) break;

            // 4. ����ִ� ���� ����Ʈ �� �ϳ��� �������� �����մϴ�.
            int randIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randIndex];

            // 5. �ش� ���� ����Ʈ�� �ε����� ã�� �׿� �´� ���� �����մϴ�.
            int pointIndex = System.Array.IndexOf(enemySpawnPoints, spawnPoint);
            string tagToSpawn = GetEnemyTagForPointIndex(pointIndex);

            SpawnEnemy(tagToSpawn, spawnPoint);

            availablePoints.RemoveAt(randIndex);
        }
    }

    // ### �߰�: ������������ ��ȿ�� ���� ����Ʈ ����� ��ȯ�ϴ� �Լ� ###
    private List<Transform> GetValidSpawnPointsForStage(int stage)
    {
        List<Transform> validPoints = new List<Transform>();

        // Enemy1 ���� ����Ʈ (3, 4, 6, 7�� -> �ε��� 2, 3, 5, 6)
        validPoints.Add(enemySpawnPoints[2]);
        validPoints.Add(enemySpawnPoints[3]);
        validPoints.Add(enemySpawnPoints[5]);
        validPoints.Add(enemySpawnPoints[6]);

        // 3������������ Enemy2 ���� ����Ʈ �߰� (2, 8�� -> �ε��� 1, 7)
        if (stage >= 3)
        {
            validPoints.Add(enemySpawnPoints[1]);
            validPoints.Add(enemySpawnPoints[7]);
        }

        // 6������������ Enemy3 ���� ����Ʈ �߰� (1, 9�� -> �ε��� 0, 8)
        if (stage >= 6)
        {
            validPoints.Add(enemySpawnPoints[0]);
            validPoints.Add(enemySpawnPoints[8]);
        }

        return validPoints;
    }

    // ### �߰�: ���� ����Ʈ �ε����� ���� �� �±׸� ��ȯ�ϴ� �Լ� ###
    private string GetEnemyTagForPointIndex(int index)
    {
        switch (index)
        {
            case 1: // 2�� ���� ����Ʈ
            case 7: // 8�� ���� ����Ʈ
                return enemyTag2;
            case 0: // 1�� ���� ����Ʈ
            case 8: // 9�� ���� ����Ʈ
                return enemyTag3;
            default: // 3, 4, 5(����), 6, 7�� ���� ����Ʈ
                return enemyTag1;
        }
    }

    // ### �߰�: �־��� ����Ʈ ��� �� ����ִ� ���� ��ȯ�ϴ� �Լ� ###
    private List<Transform> FindUnoccupiedPoints(List<Transform> pointsToCheck)
    {
        List<Transform> availablePoints = new List<Transform>(pointsToCheck);
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in activeEnemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            Transform closestPoint = null;
            float minDistance = 1.5f; // ���� �Ǵ� �ݰ��� ���� �ø�

            foreach (Transform point in pointsToCheck) // ��ü ��ȿ ����Ʈ�� ��
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

    // ### �߰�: ���� �����ϴ� �ܼ� ���� �Լ� ###
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

    // ### ���� �Լ����� ���� �״�� ���� ###
    #region ���� �Լ��� (���� ����)
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
        // ������ 5�� ���� ����Ʈ(�ε��� 4)������ ����
        if (enemySpawnPoints.Length > 4)
        {
            SpawnEnemy(bossTag, enemySpawnPoints[4]);
        }
    }
    public void NotifyItemDropped()
    {
        hasItemDroppedThisStage = true;
    }

    public IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(stageClearDelay);
        currentStage++;
        if (currentStage <= maxStage)
        {
            StartStage(currentStage);
            gameManager.LoadStage(currentStage);
        }
    }
    #endregion
}