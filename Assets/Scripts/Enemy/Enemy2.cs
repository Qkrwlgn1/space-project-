using System.Collections;
using UnityEngine;

public class Enemy2 : Enemy // ### Enemy 클래스를 상속 ###
{
    [Header("Enemy2 Pattern")]
    [SerializeField] private int burstCount = 2;       // 연속 발사 횟수
    [SerializeField] private float burstInterval = 0.2f; // 연사 간격

    // ### 수정: AutoFire 코루틴을 2연사 패턴으로 덮어쓰기(Override) ###
    protected override IEnumerator AutoFire()
    {
        // 화면에 진입할 때까지는 기본 Enemy 로직과 동일
        yield return new WaitUntil(() => hasEnteredScreen);

        // 살아있는 동안 무한 반복
        while (gameObject.activeInHierarchy && !isDead)
        {
            // 다음 연사 공격까지의 전체 딜레이 (기존 enemyFireDelay 사용)
            yield return new WaitForSeconds(enemyFireDelay);

            // 플레이어가 없다면 발사 로직을 건너뜀
            if (player == null)
            {
                continue;
            }

            // --- 2연발 발사 로직 시작 ---
            for (int i = 0; i < burstCount; i++)
            {
                Vector3 fireDirection = (player.position - bulletSpawnPoint.position).normalized;
                if (fireDirection != Vector3.zero)
                {
                    Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);
                    ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, bulletSpawnPoint.position, fireRotation);
                }

                // 각 총알 사이의 짧은 간격만큼 대기
                yield return new WaitForSeconds(burstInterval);
            }
            // --- 2연발 발사 로직 끝 ---
        }
    }
}