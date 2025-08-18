using System.Collections;
using UnityEngine;

public class Enemy3 : Enemy
{
    [Header("Enemy3 Pattern: Shotgun")]
    [SerializeField] private int bulletsPerShot;
    [SerializeField] private float spreadAngle;
    [SerializeField] private float fireDelay;

    protected override IEnumerator AutoFire()
    {
        yield return new WaitUntil(() => hasEnteredScreen);

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