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
    [SerializeField] private GameObject statusBars;


    public Transform childObject;
    public Transform bulletSpawnPointLv1;

    public EnemySpawnManager enemySpawnManager;
    public UIHPgauge uIHPgauge;

    public float bulletFireDelay;
    public int playerHealth;
    public int playerBulletDamage;
    public int playerCurrentHealth;
    public float screenPadding;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private Movement2D movement2D;



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

    public void TakeDamage(int damage)
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
            var bullet = ObjectPoolManager.instance.Pool.Get();
            bullet.transform.position = bulletSpawnPointLv1.position;

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

        statusBars.SetActive(true);

        GameManager.instance.Stop();
    }
    
}