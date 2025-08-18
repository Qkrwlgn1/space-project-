using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Stats")]
    [SerializeField] private float bossHealth = 500f;
    [SerializeField] private int burstFireDamage = 10;   // 패턴 1 데미지
    [SerializeField] private int circleShotDamage = 5;   // 패턴 2 데미지
    [SerializeField] private int whipShotDamage = 8;     // 패턴 3 데미지

    [Header("Boss Patterns")]
    [SerializeField] private float patternInterval = 3f; // 패턴 사이 대기 시간

    [Header("Pattern 2 Options")]
    [SerializeField] private int circleShotWaveCount = 3;       // 총 몇 번에 걸쳐 발사할지 (웨이브 횟수)
    [SerializeField] private int circleShotBulletCount = 20;      // 한 웨이브 당 총알 개수
    [SerializeField] private float circleShotWaveInterval = 0.5f; // 각 웨이브 사이의 시간 간격
    [SerializeField] private float circleShot_angleOffset = 30f;   // 다음 웨이브 발사 시 추가될 회전 각도
    [SerializeField] private string circleShot_bulletTag = "BossBulletPattern2";

    private int lastPatternIndex = -1;

    public override void OnEnable()
    {
        enemyHealth = bossHealth;
        base.OnEnable();
        StopAllCoroutines();
        StartCoroutine(BossPatternRoutine());
    }

    // ### 추가: 부모의 Update 함수를 덮어써서 '이동' 로직만 제거 ###
    protected override void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        // 이동(transform.position += ...) 부분은 제거하고, 회전 로직만 남깁니다.
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

        // 화면 경계를 벗어나지 않도록 하는 기능은 유지합니다.
        KeepWithinScreenBounds();
    }

    // ### 추가: 랜덤 이동 코루틴을 덮어써서 아무것도 하지 않도록 막음 ###
    protected override IEnumerator UpdateRandomMovement()
    {
        // 아무것도 하지 않고 즉시 종료하여 랜덤 이동을 원천적으로 차단합니다.
        yield break;
    }

    // ### 추가: 부모의 화면 경계 규칙을 덮어써서 화면 전체를 사용하도록 함 ###
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
        // 보스는 스폰되자마자 바로 패턴을 시작할 수 있도록 hasEnteredScreen을 true로 설정
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

    //1번째 패턴
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

    //2번째 패턴
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

    //3번째 패턴
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