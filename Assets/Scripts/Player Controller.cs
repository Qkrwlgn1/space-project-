using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigid;

    public static bool isPaused = false;

    public GameObject explosionEffectPrefab;
    public GameObject bulletPrefab;

    [Header("UI Objects")]
    public Status[] sta;

    public string explosionEffectTag = "PlayerExplosion";
    public string bulletTag = "PlayerBullet";

    public Transform childObject;
    public Transform bulletSpawnPointLv1;
    public Transform[] bulletSpawnPointLv2;
    public Transform[] bulletSpawnPointLv3;

    public EnemySpawnManager enemySpawnManager;
    public UIHPgauge uIHPgauge;

    [Header("Fire Delay Settings")]
    public float bulletFireDelay;
    public float fireDelayReduction; 
    public float minFireDelay = 0.01f;

    public float playerHealth;
    private float playerCurrentHealth;
    public float level = 1;
    public float baseDamage;
    public float screenPadding;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private Movement2D movement2D;



    void Awake()
    {
        movement2D = GetComponent<Movement2D>();
        rigid = gameObject.GetComponent<Rigidbody2D>();

    }
    void Start()
    {
        playerCurrentHealth = playerHealth;
        StartCoroutine(AutoFireBullet());

        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
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
            childObject.transform.localScale = new Vector3(2, 2, 1);
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
        uIHPgauge.UpdateGauge(playerCurrentHealth);
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
            if ((int)level <= 1)
            {
                FireBullet(bulletSpawnPointLv1);
            }
            else if ((int)level == 2)
            {
                foreach (Transform spawnPoint in bulletSpawnPointLv2)
                {

                    if (spawnPoint != null)
                    {
                        ObjectPooler.Instance.SpawnFromPool(bulletTag, spawnPoint.position, Quaternion.identity);
                    }

                }
            }
            else if ((int)level >= 3)
            {
                foreach (Transform spawnPoint in bulletSpawnPointLv3)
                {

                    ObjectPooler.Instance.SpawnFromPool(bulletTag, bulletSpawnPointLv1.position, Quaternion.identity);
                }
            }

            float currentFireDelay = bulletFireDelay - ((level - 1) * fireDelayReduction);

            currentFireDelay = Mathf.Max(currentFireDelay, minFireDelay);

            yield return new WaitForSeconds(currentFireDelay);
        }
    }


    private void FireBullet(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);

        BulletController bulletScript = bulletObj.GetComponent<BulletController>();
        if (bulletScript != null)
        {
            float calculatedDamage = baseDamage * level;

            bulletScript.playerBulletDamage = calculatedDamage;
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
            StartCoroutine(GameManager.instance.ItemSellectBars());
        }
    }

    
    public void Next()
    {
        foreach (Status status in sta)
        {
            status.gameObject.SetActive(false);
        }

        int[] ran = new int[3];

        while (true)
        {
            ran[0] = Random.Range(0, sta.Length);
            ran[1] = Random.Range(0, sta.Length);
            ran[2] = Random.Range(0, sta.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
            {
                Debug.Log("Date existing");
                break;
            }
        }
        
        for (int i = 0; i < ran.Length; i++)
        {
            Status ranSta = sta[ran[i]];

            if (ranSta.level == 5)
            {
                ranSta.gameObject.SetActive(false);
            }
            else
            {
                ranSta.gameObject.SetActive(true);
            }
        }
    }
    
}