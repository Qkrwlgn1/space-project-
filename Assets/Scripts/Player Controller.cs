using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Rigidbody2D rigid;
    public Transform childObject;
    public Transform bulletSpawnPointLv1;
    public Transform[] bulletSpawnPointLv2;
    public Transform[] bulletSpawnPointLv3;
    public EnemySpawnManager enemySpawnManager;
    public UIHPgauge uIHPgauge;

    [Header("UI Objects")]
    public Status[] sta;

    [Header("Pool Tags")]
    public string explosionEffectTag = "PlayerExplosion";
    public string bulletTag = "PlayerBullet";


    [Header("Player Stats")]
    public float playerHealth;
    public int playerDamageLevel = 1;
    public int playerSpeedLevel = 1;
    public int playerHPLevel = 1;
    public int playerDelayLevel = 1;
    public int playerBulletLevel = 1;
    public int playerBulletSizeLevel = 1;

    [Header("Upgrade Values")]
    public float baseDamage = 10f;
    public float speedIncreasePerLevel = 0.5f;
    public float healthIncreasePerLevel = 20f;
    public float sizeIncreasePerLevel = 0.2f;

    [Header("Fire Delay Settings")]
    public float baseFireDelay = 0.5f;
    public float fireDelayReduction = 0.05f;
    public float minFireDelay = 0.1f;

    [Header("Invincible Settings")]
    public float invincibleTime = 2f;
    public bool isInvincible = false;
    public float blinkInterval = 0.1f;

    [Header("Screen Bounds")]
    public float screenPadding;

    public float playerCurrentHealth;
    private int playerLayer;
    private int invincibleLayer;
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
        if (uIHPgauge != null) uIHPgauge.SetMaxHealth(playerHealth);
        StartCoroutine(AutoFireBullet());
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
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
            if (childObject != null) childObject.transform.localScale = new Vector3(1, 1, 1);
            if (movement2D.moveSpeed >= movement2D.minSpeed) movement2D.moveSpeed--;
        }
        else
        {
            if (childObject != null) childObject.transform.localScale = new Vector3(2, 2, 1);
        }
        if (y > 0)
        {
            if (movement2D.moveSpeed < 7f) movement2D.moveSpeed++;
        }
        movement2D.MoveTo(new Vector3(x, y, 0));
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;
        playerCurrentHealth -= damage;
        AudioManagerScript.Instance.PlayerSFX(0);
        uIHPgauge.UpdateGauge(playerCurrentHealth);
        StartCoroutine(Invincible());
        if (playerCurrentHealth <= 0)
        {
            AudioManagerScript.Instance.PlayerSFX(1);
            Die();
        }
    }

    private void Die()
    {
        ObjectPooler.Instance.SpawnFromPool(explosionEffectTag, transform.position, Quaternion.identity);
        if (enemySpawnManager != null) enemySpawnManager.isPlayerAlive = false;
        gameObject.SetActive(false);
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
            if (spriteRen != null && spriteRen.Length > 0)
            {
                foreach (SpriteRenderer renderer in spriteRen) { if (renderer != null) renderer.color = new Color(1, 1, 1, 0.5f); }
                yield return new WaitForSeconds(blinkInterval);
                foreach (SpriteRenderer renderer in spriteRen) { if (renderer != null) renderer.color = new Color(1, 1, 1, 1f); }
                yield return new WaitForSeconds(blinkInterval);
            }
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

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < sta.Length; i++)
        {
            if (sta[i].level < sta[i].data.maxLevel)
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count < 3)
        {
            foreach (int index in availableIndices)
            {
                sta[index].gameObject.SetActive(true);
            }
        }
        else
        {
            int[] ran = new int[3];
            while (true)
            {
                ran[0] = availableIndices[Random.Range(0, availableIndices.Count)];
                ran[1] = availableIndices[Random.Range(0, availableIndices.Count)];
                ran[2] = availableIndices[Random.Range(0, availableIndices.Count)];

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                {
                    break;
                }
            }

            for (int i = 0; i < ran.Length; i++)
            {
                sta[ran[i]].gameObject.SetActive(true);
            }
        }
    }

    public void UpgradePlayerDamage() { playerDamageLevel++; }
    public void UpgradePlayerSpeed() { playerSpeedLevel++; if (movement2D != null) movement2D.moveSpeed += speedIncreasePerLevel; }
    public void UpgradePlayerHP() { playerHPLevel++; playerHealth += healthIncreasePerLevel; playerCurrentHealth += healthIncreasePerLevel; if (uIHPgauge != null) { uIHPgauge.slider.maxValue = playerHealth; uIHPgauge.UpdateGauge(playerCurrentHealth); } }
    public void UpgradePlayerBulletLevel() { playerBulletLevel++; }
    public void UpgradePlayerBulletSize() { playerBulletSizeLevel++; }
    public void UpgradePlayerDelay() { playerDelayLevel++; }

    public int GetCurrentStatLevel(StatusData.StatusType type)
    {
        switch (type)
        {
            case StatusData.StatusType.Damage: return playerDamageLevel;
            case StatusData.StatusType.Speed: return playerSpeedLevel;
            case StatusData.StatusType.HP: return playerHPLevel;
            case StatusData.StatusType.WeaponNumber: return playerBulletLevel;
            case StatusData.StatusType.BulletSize: return playerBulletSizeLevel;
            case StatusData.StatusType.Delay: return playerDelayLevel;
            default: return 1;
        }
    }

    private IEnumerator AutoFireBullet()
    {
        while (true)
        {
            if (GameManager.instance != null && !GameManager.instance.isLive) { yield return null; continue; }

            if (playerBulletLevel <= 1) FireBullet(bulletSpawnPointLv1);
            else if (playerBulletLevel == 2) { foreach (Transform sp in bulletSpawnPointLv2) FireBullet(sp); }
            else if (playerBulletLevel >= 3) { foreach (Transform sp in bulletSpawnPointLv3) FireBullet(sp); }

            float currentFireDelay = baseFireDelay - ((playerDelayLevel - 1) * fireDelayReduction);
            currentFireDelay = Mathf.Max(currentFireDelay, minFireDelay);
            yield return new WaitForSeconds(currentFireDelay);
        }
    }

    private void FireBullet(Transform spawnPoint)
    {
        AudioManagerScript.Instance.PlayerSFX(2);

        if (spawnPoint == null) return;
        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(bulletTag, spawnPoint.position, Quaternion.identity);
        if (bulletObj == null) return;
        BulletController bulletScript = bulletObj.GetComponent<BulletController>();
        if (bulletScript != null)
        {
            bulletScript.playerBulletDamage = baseDamage * playerDamageLevel;
            float calculatedSize = 1f + ((playerBulletSizeLevel - 1) * sizeIncreasePerLevel);
            bulletScript.SetSize(calculatedSize);
        }
    }
}