using System.Collections;
using UnityEngine;

public class Enemy2 : Enemy
{
    [Header("Enemy2 Pattern")]
    [SerializeField] private int burstCount = 2;
    [SerializeField] private float burstInterval = 0.2f;

    [Header("Enemy2 Movement")]
    [SerializeField] private float entrySpeed = 2f;

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
        isEntering = true;
        entryStartPoint = transform.position;
        entryEndPoint = new Vector3(-entryStartPoint.x * 0.8f, screenBounds.y * 0.7f, 0);
        transform.rotation = Quaternion.Euler(0, 0, transform.position.x > 0 ? -90 : 90);
        entryTimer = 0f;

        StartCoroutine(AutoFire());

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

        StartCoroutine(base.UpdateRandomMovement());
    }
    protected override void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        if (!isEntering)
        {
            base.Update();
        }
    }
    protected override IEnumerator AutoFire()
    {
        while (gameObject.activeInHierarchy && !isDead)
        {
            yield return new WaitForSeconds(enemyFireDelay);
            if (player == null) continue;

            for (int i = 0; i < burstCount; i++)
            {
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