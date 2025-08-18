using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Stats")]
    [SerializeField] private float bossHealth = 500f;
    [SerializeField] private int burstFireDamage = 10;
    [SerializeField] private int circleShotDamage = 5;
    [SerializeField] private int whipShotDamage = 8;

    [Header("Boss Patterns")]

    [SerializeField] private float patternInterval = 3f;

    [Header("Pattern 2 Options")]
    [SerializeField] private int circleShotWaveCount = 3;
    [SerializeField] private int circleShotBulletCount = 20;
    [SerializeField] private float circleShotWaveInterval = 0.5f;
    [SerializeField] private float circleShot_angleOffset = 30f;
    [SerializeField] private string circleShot_bulletTag = "BossBulletPattern2";

    private int lastPatternIndex = -1;

    public override void OnEnable()
    {
        enemyHealth = bossHealth;
        base.OnEnable();
        StopAllCoroutines();
        StartCoroutine(BossPatternRoutine());
    }

    // ### �߰�: �θ��� Update �Լ��� ����Ἥ '�̵�' ������ ���� ###
    protected override void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        // �̵�(transform.position += ...) �κ��� �����ϰ�, ȸ�� ������ ����ϴ�.
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

        // ȭ�� ��踦 ����� �ʵ��� �ϴ� ����� �����մϴ�.
        KeepWithinScreenBounds();
    }

    // ### �߰�: ���� �̵� �ڷ�ƾ�� ����Ἥ �ƹ��͵� ���� �ʵ��� ���� ###
    protected override IEnumerator UpdateRandomMovement()
    {
        // �ƹ��͵� ���� �ʰ� ��� �����Ͽ� ���� �̵��� ��õ������ �����մϴ�.
        yield break;
    }

    // ### �߰�: �θ��� ȭ�� ��� ��Ģ�� ����Ἥ ȭ�� ��ü�� ����ϵ��� �� ###
    protected override void KeepWithinScreenBounds()
    {
        Vector3 pos = transform.position;
        bool needsRepositioning = false;

        if (pos.x < -screenBounds.x + screenBoundsPadding) { pos.x = -screenBounds.x + screenBoundsPadding; needsRepositioning = true; }
        else if (pos.x > screenBounds.x + screenBoundsPadding) { pos.x = screenBounds.x - screenBoundsPadding; needsRepositioning = true; }

        if (pos.y < -screenBounds.y + screenBoundsPadding) { pos.y = -screenBounds.y + screenBoundsPadding; needsRepositioning = true; }
        else if (pos.y > screenBounds.y - screenBoundsPadding) { pos.y = screenBounds.y - screenBoundsPadding; needsRepositioning = true; }

        if (needsRepositioning)
        {
            transform.position = pos;
        }
    }

    private IEnumerator BossPatternRoutine()
    {
        // ������ �������ڸ��� �ٷ� ������ ������ �� �ֵ��� hasEnteredScreen�� true�� ����
        hasEnteredScreen = true;

        while (gameObject.activeInHierarchy && !isDead)
        {
            int patternIndex;
            do
            {
                patternIndex = Random.Range(0, 3);
            } while (patternIndex == lastPatternIndex);

            lastPatternIndex = patternIndex;

            switch (patternIndex)
            {
                case 0:
                    yield return StartCoroutine(BurstFirePattern());
                    break;
                case 1:
                    yield return StartCoroutine(CircleShotPattern());
                    break;
                case 2:
                    yield return StartCoroutine(WhipShotPattern());
                    break;
            }

            yield return new WaitForSeconds(patternInterval);
        }
    }

    //1��° ����
    private IEnumerator BurstFirePattern()
    {
        if (player == null) yield break;

        int burstCount = 4;
        float burstInterval = 0.25f;

        for (int i = 0; i < burstCount; i++)
        {
            Vector3 fireDirection = (player.position - bulletSpawnPoint.position).normalized;
            Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);

            FireBulletWithDamage(enemyBulletTag, burstFireDamage, bulletSpawnPoint.position, fireRotation);

            yield return new WaitForSeconds(burstInterval);
        }
    }

    //2��° ����
    private IEnumerator CircleShotPattern()
    {
        float currentAngleOffset = 0f;
        float angleStep = 360f / circleShotBulletCount;

        for (int w = 0; w < circleShotWaveCount; w++)
        {
            for (int i = 0; i < circleShotBulletCount; i++)
            {
                float angle = (i * angleStep) + currentAngleOffset;
                Quaternion fireRotation = Quaternion.Euler(0, 0, angle);

                FireBulletWithDamage(circleShot_bulletTag, circleShotDamage, bulletSpawnPoint.position, fireRotation);
            }

            currentAngleOffset += circleShot_angleOffset;

            if (w < circleShotWaveCount - 1)
            {
                yield return new WaitForSeconds(circleShotWaveInterval);
            }
        }

        yield return null;
    }

    //3��° ����
    private IEnumerator WhipShotPattern()
    {
        int bulletCount = 15;
        float whipInterval = 0.1f;

        for (int i = 0; i < bulletCount; i++)
        {
            float xPos = Mathf.Cos(Mathf.PI * 2 * i / bulletCount);
            Vector3 fireDirection = new Vector3(xPos, -1, 0).normalized;

            Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);

            FireBulletWithDamage(enemyBulletTag, whipShotDamage, bulletSpawnPoint.position, fireRotation);

            yield return new WaitForSeconds(whipInterval);
        }
    }

    private void FireBulletWithDamage(string bulletTag, int damage, Vector3 position, Quaternion rotation)
    {
        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(bulletTag, position, rotation);
        if (bulletObj != null)
        {
            EnemyBulletController bulletController = bulletObj.GetComponent<EnemyBulletController>();
            if (bulletController != null)
            {
                bulletController.enemyBulletDamage = damage;
            }
        }
    }
}