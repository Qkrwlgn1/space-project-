using System.Collections;
using UnityEngine;

public class Enemy2 : Enemy
{
    [Header("Enemy2 Pattern")]
    [SerializeField] private int burstCount = 2;
    [SerializeField] private float burstInterval = 0.2f;

    [Header("Enemy2 Movement")]
    [SerializeField] private float entrySpeed = 2f; // 진입하는 데 걸리는 시간(초)

    private bool isEntering;
    private Vector3 entryStartPoint;
    private Vector3 entryEndPoint;
    private float entryTimer;

    public override void OnEnable()
    {
        base.OnEnable();
        StopAllCoroutines();
        StartCoroutine(MainBehaviorRoutine());
    }

    private IEnumerator MainBehaviorRoutine()
    {
        // --- 1. 화면 진입 설정 ---
        isEntering = true;
        entryStartPoint = transform.position;
        entryEndPoint = new Vector3(-entryStartPoint.x * 0.8f, screenBounds.y * 0.7f, 0);
        transform.rotation = Quaternion.Euler(0, 0, transform.position.x > 0 ? -90 : 90);
        entryTimer = 0f;

        // ### 공격 코루틴을 진입 이동과 '동시에' 시작 ###
        StartCoroutine(AutoFire());

        // --- 2. 화면 진입 이동 실행 ---
        while (isEntering)
        {
            float progress = entryTimer / entrySpeed;
            transform.position = Vector3.Lerp(entryStartPoint, entryEndPoint, progress);
            entryTimer += Time.deltaTime;
            if (progress >= 1f)
            {
                isEntering = false;
            }
            yield return null;
        }

        // --- 3. 일반 랜덤 이동 시작 ---
        StartCoroutine(base.UpdateRandomMovement());
    }

    protected override void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        // 진입이 끝난 후에는 부모의 Update 로직(플레이어 추적 회전 등)을 사용
        if (!isEntering)
        {
            base.Update();
        }
    }

    protected override IEnumerator AutoFire()
    {
        // 이제 진입 여부와 상관없이 바로 발사 로직을 시작할 수 있음
        while (gameObject.activeInHierarchy && !isDead)
        {
            yield return new WaitForSeconds(enemyFireDelay);
            if (player == null) continue;

            for (int i = 0; i < burstCount; i++)
            {
                // 진입 중에는 기체 정면으로, 진입 후에는 플레이어를 조준하도록 수정
                Quaternion fireRotation;
                if (isEntering)
                {
                    fireRotation = transform.rotation;
                }
                else
                {
                    Vector3 fireDirection = (player.position - bulletSpawnPoint.position).normalized;
                    fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);
                }

                ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, bulletSpawnPoint.position, fireRotation);

                if (i < burstCount - 1)
                {
                    yield return new WaitForSeconds(burstInterval);
                }
            }
        }
    }
}