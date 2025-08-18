using System.Collections;
using UnityEngine;

public class Enemy3 : Enemy
{
    [Header("Enemy3 Pattern: Shotgun")]
    [SerializeField] private int bulletsPerShot = 5;
    [SerializeField] private float spreadAngle = 45f;
    [SerializeField] private float fireDelay = 2f;

    [Header("Enemy3 Entry Movement")]
    [SerializeField] private float entrySpeed = 3f;

    private bool isEntering;
    private Vector3 entryDestination;

    public override void OnEnable()
    {
        base.OnEnable();
        StopAllCoroutines();

        isEntering = true;

        float destinationX = (transform.position.x > 0) ? -screenBounds.x + screenBoundsPadding : screenBounds.x - screenBoundsPadding;
        float destinationY = -screenBounds.y + screenBoundsPadding;

        entryDestination = new Vector3(destinationX, destinationY, 0);

        Vector3 direction = (entryDestination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);
        }

        StartCoroutine(AutoFire());
    }
    protected override void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;

        if (isEntering)
        {
            transform.position = Vector3.MoveTowards(transform.position, entryDestination, entrySpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, entryDestination) < 0.1f)
            {
                isEntering = false;
                StartCoroutine(base.UpdateRandomMovement());
            }
        }
        else
        {
            base.Update();
        }
    }
    protected override IEnumerator AutoFire()
    {
        yield return new WaitUntil(() => !isEntering);

        while (gameObject.activeInHierarchy && !isDead)
        {
            yield return new WaitForSeconds(fireDelay);

            if (player == null) continue;

            Vector3 directionToPlayer = (player.position - bulletSpawnPoint.position).normalized;
            float centerAngle = Mathf.Atan2(-directionToPlayer.x, directionToPlayer.y) * Mathf.Rad2Deg;
            float oppositeAngle = centerAngle + 180f;

            float startAngle = oppositeAngle - spreadAngle / 2;
            float angleStep = spreadAngle / (bulletsPerShot - 1);

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Quaternion fireRotation = Quaternion.Euler(0, 0, currentAngle);

                ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, bulletSpawnPoint.position, fireRotation);
            }
        }
    }
}