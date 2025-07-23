using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ... 기존 변수들은 그대로 ...
    [Header("Enemy Stats")]
    public float enemyHealth;
    private float enemyCurrentHealth;

    [Header("Prefabs")]
    public GameObject explosionEffectPrefab;
    public GameObject enemyBulletPrefab;
    public GameObject dropItemPrefab;

    [Header("Combat")]
    public float enemyFireDelay = 1f;
    public float predictionTime = 0.5f;
    public bool isDropItem = false;
    public Transform bulletSpawnPoint;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 150f;
    public float minMoveWaitTime = 1f;
    public float maxMoveWaitTime = 3f;

    // ### 추가된 변수들 ###
    [Header("Avoidance")]
    public LayerMask enemyLayer; // 감지할 적군 레이어 (Inspector에서 Enemy로 설정)
    public float avoidanceRadius = 1.0f; // 회피를 시작할 반경
    public float avoidanceForce = 5.0f; // 서로를 밀어내는 힘의 크기

    [Header("Screen Bounds")]
    public float screenBoundsPadding = 0.5f;

    public EnemySpawnManager spawnManager;

    private Camera mainCamera;
    private Vector3 screenBounds;
    private bool hasEnteredScreen = false;

    private Transform player;
    private Vector3 moveDestination;
    private Vector3 gizmoTargetPosition;

    void Start()
    {
        // ... Start() 메서드는 기존과 동일 ...
        enemyCurrentHealth = enemyHealth;
        mainCamera = Camera.main;
        CalculateScreenBounds();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        StartCoroutine(AutoFire());
        StartCoroutine(UpdateRandomMovement());
    }

    void Update()
    {
        if (!hasEnteredScreen)
        {
            if (moveDestination == Vector3.zero)
                moveDestination = transform.position;

            MoveToEnterScreen();
        }
        else
        {
            // ### Update 로직 수정 ###

            // 1. 기본 이동 방향 계산 (랜덤 목적지)
            Vector3 randomMoveDirection = (moveDestination - transform.position).normalized;

            // 2. 다른 적군을 피하기 위한 회피 방향 계산
            Vector3 avoidanceDirection = CalculateAvoidanceVector();

            // 3. 두 방향을 조합하여 최종 이동 방향 결정
            // 회피 방향에 가중치(avoidanceForce)를 주어 더 강하게 영향을 받도록 함
            Vector3 finalDirection = (randomMoveDirection + avoidanceDirection * avoidanceForce).normalized;

            // 4. 최종 방향으로 이동
            transform.position += finalDirection * moveSpeed * Time.deltaTime;

            // 5. 플레이어 바라보기 (회전 로직은 동일)
            if (player != null)
            {
                gizmoTargetPosition = player.position;
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, -directionToPlayer);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    /// <summary>
    /// 주변의 다른 적들을 감지하고, 그들로부터 멀어지는 회피 벡터를 계산합니다.
    /// </summary>
    /// <returns>최종 회피 방향 벡터</returns>
    private Vector3 CalculateAvoidanceVector()
    {
        Vector3 avoidanceVector = Vector3.zero;
        int nearbyEnemies = 0;

        // 자신의 avoidanceRadius 반경 안에 있는 모든 'Enemy' 레이어의 콜라이더를 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, enemyLayer);

        foreach (var col in colliders)
        {
            // 감지된 것이 자기 자신이 아닐 경우
            if (col.gameObject != this.gameObject)
            {
                // 다른 적으로부터 멀어지는 방향을 계산하여 누적
                avoidanceVector += (transform.position - col.transform.position);
                nearbyEnemies++;
            }
        }

        if (nearbyEnemies > 0)
        {
            // 평균적인 회피 방향을 계산
            avoidanceVector /= nearbyEnemies;
        }

        return avoidanceVector.normalized;
    }

    // ... 나머지 메서드들은 기존과 동일하게 유지됩니다 ...
    private IEnumerator UpdateRandomMovement()
    {
        yield return new WaitUntil(() => hasEnteredScreen);
        moveDestination = transform.position;
        while (true)
        {
            float randomX = Random.Range(-screenBounds.x + screenBoundsPadding, screenBounds.x - screenBoundsPadding);
            float randomY = Random.Range(0, screenBounds.y - screenBoundsPadding);
            moveDestination = new Vector3(randomX, randomY, 0);
            float waitTime = Random.Range(minMoveWaitTime, maxMoveWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void MoveToEnterScreen()
    {
        Vector3 screenCenter = Vector3.zero;
        screenCenter.y = screenBounds.y * 0.7f;
        transform.position = Vector3.MoveTowards(transform.position, screenCenter, moveSpeed * Time.deltaTime);
        if (IsWithinScreenBounds())
        {
            hasEnteredScreen = true;
        }
    }

    bool IsWithinScreenBounds()
    {
        Vector3 pos = transform.position;
        return pos.x >= -screenBounds.x + screenBoundsPadding &&
               pos.x <= screenBounds.x - screenBoundsPadding &&
               pos.y >= 0 &&
               pos.y <= screenBounds.y - screenBoundsPadding;
    }

    void CalculateScreenBounds()
    {
        if (mainCamera == null) return;
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));
    }

    public void TakeDamage(float damage)
    {
        enemyCurrentHealth -= damage;
        Debug.Log("Enemy hit!  HP : " + enemyCurrentHealth);
        if (enemyCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        if (isDropItem && dropItemPrefab != null)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
        if (spawnManager != null)
        {
            spawnManager.OnEnemyKilled();
        }
        Destroy(gameObject);
    }

    private IEnumerator AutoFire()
    {
        yield return new WaitUntil(() => hasEnteredScreen);
        while (true)
        {
            if (player != null)
            {
                Vector3 aimPosition = player.position;
                Vector3 fireDirection = (aimPosition - bulletSpawnPoint.position).normalized;
                Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);
                yield return new WaitForSeconds(predictionTime);
                if (enemyBulletPrefab != null && bulletSpawnPoint != null)
                {
                    Instantiate(enemyBulletPrefab, bulletSpawnPoint.position, fireRotation);
                }
                float remainingWaitTime = enemyFireDelay - predictionTime;
                if (remainingWaitTime > 0)
                {
                    yield return new WaitForSeconds(remainingWaitTime);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;
        Gizmos.color = Color.red;
        Vector3 boundsCenter = new Vector3(0, screenBounds.y / 2, 0);
        Vector3 boundsSize = new Vector3((screenBounds.x - screenBoundsPadding) * 2, screenBounds.y, 0);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(moveDestination, 0.3f);
        Gizmos.DrawLine(transform.position, moveDestination);

        // 회피 반경 시각화 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, gizmoTargetPosition);
        }
    }
}