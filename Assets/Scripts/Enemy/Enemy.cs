using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float enemyHealth;
    private float enemyCurrentHealth;

    [Header("Pool Tags")]
    public string explosionEffectTag = "EnemyExplosion";
    public string enemyBulletTag = "EnemyBullet";
    public string dropItemTag = "Item";

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

    [Header("Avoidance")]
    public LayerMask enemyLayer;
    public float avoidanceRadius = 1.0f;
    public float avoidanceForce = 5.0f;

    [Header("Screen Bounds")]
    public float screenBoundsPadding = 0.5f;

    public EnemySpawnManager spawnManager;

    private Camera mainCamera;
    private Vector3 screenBounds;
    protected bool hasEnteredScreen = false;

    protected Transform player;
    private Vector3 moveDestination;
    private Vector3 gizmoTargetPosition;

    protected bool isDead = false;

    public virtual void OnEnable()
    {
        enemyCurrentHealth = enemyHealth;
        hasEnteredScreen = false;
        isDead = false;

        if (mainCamera == null) mainCamera = Camera.main;
        CalculateScreenBounds();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        StartCoroutine(AutoFire());
        StartCoroutine(UpdateRandomMovement());
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        if (!hasEnteredScreen)
        {
            if (moveDestination == Vector3.zero)
                moveDestination = transform.position;

            MoveToEnterScreen();
        }
        else
        {
            Vector3 randomMoveDirection = (moveDestination - transform.position).normalized;
            Vector3 avoidanceDirection = CalculateAvoidanceVector();
            Vector3 finalDirection = (randomMoveDirection + avoidanceDirection * avoidanceForce).normalized;
            transform.position += finalDirection * moveSpeed * Time.deltaTime;

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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        enemyCurrentHealth -= damage;
        Debug.Log("Enemy hit!  HP : " + enemyCurrentHealth);
        if (enemyCurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GameObject effect = ObjectPooler.Instance.SpawnFromPool(explosionEffectTag, transform.position, Quaternion.identity);
        StartCoroutine(DisableAfterTime(effect, 2f));

        if (isDropItem)
        {
            SpawnItem();
        }

        if (spawnManager != null)
        {
            spawnManager.OnEnemyKilled(transform.position);
        }

        gameObject.SetActive(false);
    }

    private void SpawnItem()
    {
        if (spawnManager != null && !spawnManager.HasItemDroppedThisStage)
        {
            if (Random.Range(0, 100) < 10)
            {
                ObjectPooler.Instance.SpawnFromPool(dropItemTag, transform.position, Quaternion.identity);
                spawnManager.NotifyItemDropped();
            }
        }
    }

    protected virtual IEnumerator AutoFire()
    {
        yield return new WaitUntil(() => hasEnteredScreen);
        while (gameObject.activeInHierarchy)
        {
            if (player != null)
            {
                Vector3 aimPosition = player.position;
                Vector3 fireDirection = (aimPosition - bulletSpawnPoint.position).normalized;
                if (fireDirection != Vector3.zero)
                {
                    Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);
                    yield return new WaitForSeconds(predictionTime);

                    ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, bulletSpawnPoint.position, fireRotation);
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

    private IEnumerator DisableAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }

    private Vector3 CalculateAvoidanceVector()
    {
        Vector3 avoidanceVector = Vector3.zero;
        int nearbyEnemies = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, enemyLayer);
        foreach (var col in colliders)
        {
            if (col.gameObject != this.gameObject)
            {
                avoidanceVector += (transform.position - col.transform.position);
                nearbyEnemies++;
            }
        }
        if (nearbyEnemies > 0)
        {
            avoidanceVector /= nearbyEnemies;
        }
        return avoidanceVector.normalized;
    }

    public virtual IEnumerator UpdateRandomMovement()
    {
        yield return new WaitUntil(() => hasEnteredScreen);
        moveDestination = transform.position;
        while (gameObject.activeInHierarchy)
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
}