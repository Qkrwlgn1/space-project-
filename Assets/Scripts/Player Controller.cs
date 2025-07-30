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
    [SerializeField] private GameObject itemBack;
    [SerializeField] private GameObject spawnBars;


    public Transform childObject;
    public Transform bulletSpawnPointLv1;

    public EnemySpawnManager enemySpawnManager;
    public UIHPgauge uIHPgauge;

    public float bulletFireDelay;
    public float playerHealth;
    private float playerCurrentHealth;
    public float level = 1;
    public float screenPadding;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private Movement2D movement2D;
    private IObjectPool<BulletController> _Pool;


    void Awake()
    {
        _Pool = new ObjectPool<BulletController>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 10);

    }

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
            childObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            childObject.transform.localScale = new Vector3(2, 2, 1);
        }

        if (y < 0)
        {
            if (movement2D.moveSpeed >= movement2D.minSpeed)
            {
                movement2D.moveSpeed--;
            }
        }
        else if (y > 0)
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
        GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        if (enemySpawnManager != null)
        {
            enemySpawnManager.isPlayerAlive = false;
        }
        Destroy(gameObject);
    }

    

    private IEnumerator AutoFireBullet()
    {
        while (true)
        {
            var bullet = _Pool.Get();

            yield return new WaitForSeconds(bulletFireDelay);
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


    private BulletController CreateBullet()
    {
        BulletController bullet = Instantiate(bulletPrefab, bulletSpawnPointLv1.position, Quaternion.identity).GetComponent<BulletController>();
        bullet.SetManagePool(_Pool);
        return bullet;
    }

    private void OnGetBullet(BulletController bullet)
    {
        bullet.gameObject.SetActive(true);

    }

    private void OnReleaseBullet(BulletController bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(BulletController bullet)
    {
        Destroy(bullet.gameObject);
    }
}