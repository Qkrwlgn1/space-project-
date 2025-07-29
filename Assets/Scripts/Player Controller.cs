using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigid;

    public string explosionEffectTag = "PlayerExplosion";
    public string bulletTag = "PlayerBullet";

    public Transform childObject;
    public Transform bulletSpawnPointLv1;
    public Transform[] bulletSpawnPointLv2;
    public EnemySpawnManager enemySpawnManager;

    public float bulletFireDelay;
    public float playerHealth;
    private float playerCurrentHealth;
    public float level = 1;
    public float screenPadding;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private Movement2D movement2D;

    [Header("UI Objects")]
    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject spawnBars;

    void Start()
    {
        movement2D = GetComponent<Movement2D>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
        playerCurrentHealth = playerHealth;
        StartCoroutine(AutoFireBullet());

        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void Update()
    {
        Move();
    }

    void LateUpdate()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + screenPadding, screenBounds.x - screenPadding);
        viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1 + screenPadding, screenBounds.y - screenPadding);
        transform.position = viewPos;
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (y < 0)
        {
            if (childObject != null)
            {
                childObject.transform.localScale = new Vector3(1, 1, 1);
            }
            if (movement2D.moveSpeed >= movement2D.minSpeed)
            {
                movement2D.moveSpeed--;
            }
        }
        else
        {
            if (childObject != null)
            {
                childObject.transform.localScale = new Vector3(2, 2, 1);
            }
        }
        if (y > 0)
        {
            if (movement2D.moveSpeed < 7f)
            {
                movement2D.moveSpeed++;
            }
        }
        movement2D.MoveTo(new Vector3(x, y, 0));
    }

    public void TakeDamage(float damage)
    {
        playerCurrentHealth -= damage;
        Debug.Log("Player Health : " + playerCurrentHealth);
        if (playerCurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameObject effect = ObjectPooler.Instance.SpawnFromPool(explosionEffectTag, transform.position, Quaternion.identity);
        StartCoroutine(DisableAfterTime(effect, 2f));

        if (enemySpawnManager != null)
        {
            enemySpawnManager.isPlayerAlive = false;
        }

        gameObject.SetActive(false);
    }

    public void LevelUp()
    {
        level++;
        Debug.Log("플레이어 레벨 1 상승 ! 현재 레벨 : " + level);
    }

    private IEnumerator AutoFireBullet()
    {
        while (true)
        {
            if (level >= 2)
            {
                foreach (Transform spawnPoint in bulletSpawnPointLv2)
                {
                    if (spawnPoint != null)
                    {
                        ObjectPooler.Instance.SpawnFromPool(bulletTag, spawnPoint.position, Quaternion.identity);
                    }
                }
            }
            else
            {
                if (bulletSpawnPointLv1 != null)
                {
                    ObjectPooler.Instance.SpawnFromPool(bulletTag, bulletSpawnPointLv1.position, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(bulletFireDelay);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {

            StartCoroutine(ItemSellectBars());
        }
    }

    IEnumerator ItemSellectBars()
    {
        itemBack.SetActive(true);

        yield return new WaitForSeconds(1f);

        spawnBars.SetActive(true);
    }

    public void DeActiveItemSellectBars()
    {
        itemBack.SetActive(false);
        spawnBars.SetActive(false);
    }
}