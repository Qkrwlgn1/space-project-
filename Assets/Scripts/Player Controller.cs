using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigid;

    public static bool isPaused = false;

    [Header("UI Objects")]
    public Status[] sta;

    [Header("Pool Tags")]
    public string explosionEffectTag = "PlayerExplosion";
    public string bulletTag = "PlayerBullet";

    [Header("Object References")]
    public Transform childObject;
    public Transform bulletSpawnPointLv1;
    public Transform[] bulletSpawnPointLv2;
    public Transform[] bulletSpawnPointLv3;
    public EnemySpawnManager enemySpawnManager;
    [SerializeField] private UIHPgauge uIHPgauge;

    [Header("Stats")]
    public float playerHealth = 10;
    public float level = 1;
    public float baseDamage;
    public float playerCurrentHealth;
    public float invincibleTime;
    public bool isInvincible = false;
    private int playerLayer;
    private int invincibleLayer;
    public float blinkInterval = 0.1f;

    [Header("Fire Delay Settings")]
    public float bulletFireDelay;
    public float fireDelayReduction;
    public float minFireDelay = 0.01f;

    [Header("Screen Bounds")]
    public float screenPadding;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private Movement2D movement2D;
    private SpriteRenderer[] spriteRen;

    void Awake()
    {
        movement2D = GetComponent<Movement2D>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
        spriteRen = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        invincibleLayer = LayerMask.NameToLayer("InvinciblePlayer");
        playerCurrentHealth = playerHealth;
        StartCoroutine(AutoFireBullet());

        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
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
                childObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (childObject != null)
                childObject.transform.localScale = new Vector3(2, 2, 1);
        }

        if (y < 0)
        {
            if (movement2D.moveSpeed >= movement2D.minSpeed)
                movement2D.moveSpeed--;
        }
        else if (y > 0)
        {
            if (movement2D.moveSpeed < 7f)
                movement2D.moveSpeed++;
        }
        movement2D.MoveTo(new Vector3(x, y, 0));
    }

    public void TakeDamage(float damage)
    {
        playerCurrentHealth -= damage;
        uIHPgauge.UpdateGauge(playerCurrentHealth);
        StartCoroutine(Invincible());
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
    }

    private IEnumerator AutoFireBullet()
    {
        while (true)
        {
            if (GameManager.instance != null && !GameManager.instance.isLive)
            {
                yield return null;
                continue;
            }

            if ((int)level <= 1)
            {
                FireBullet(bulletSpawnPointLv1);
            }
            else if ((int)level == 2)
            {
                foreach (Transform spawnPoint in bulletSpawnPointLv2)
                {
                    FireBullet(spawnPoint);
                }
            }
            else if ((int)level >= 3)
            {
                foreach (Transform spawnPoint in bulletSpawnPointLv3)
                {
                    FireBullet(spawnPoint);
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

        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(bulletTag, spawnPoint.position, Quaternion.identity);

        if (bulletObj == null) return;

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
        EnemyBulletController enemyBullet = collision.GetComponent<EnemyBulletController>();
        if (collision.CompareTag("Item"))
        {
            if (GameManager.instance != null)
            {
                StartCoroutine(GameManager.instance.ItemSellectBars());
            }
        }

        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(enemyBullet.enemyBulletDamage);
        }
        else if (collision.CompareTag("EnemyBullet") && !isInvincible)
        {
            TakeDamage(enemyBullet.enemyBulletDamage);
        }
    }

    public IEnumerator Invincible()
    {
        isInvincible = true;
        gameObject.layer = invincibleLayer;
        float elapsedTime = 0f;

        while (elapsedTime < invincibleTime)
        {
            foreach (SpriteRenderer renderer in spriteRen)
            {
                Color color = renderer.color;
                color.a = 0f;
                renderer.color = color;
            }

            yield return new WaitForSeconds(blinkInterval);

            foreach (SpriteRenderer renderer in spriteRen)
            {
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }

            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        foreach (SpriteRenderer renderer in spriteRen)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }

        gameObject.layer = playerLayer;
        isInvincible = false;
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