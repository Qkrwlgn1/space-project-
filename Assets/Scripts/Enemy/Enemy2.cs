using System.Collections;
using UnityEngine;

public class Enemy2 : Enemy // ### Enemy Ŭ������ ��� ###
{
    [Header("Enemy2 Pattern")]
    [SerializeField] private int burstCount = 2;       // ���� �߻� Ƚ��
    [SerializeField] private float burstInterval = 0.2f; // ���� ����

    // ### ����: AutoFire �ڷ�ƾ�� 2���� �������� �����(Override) ###
    protected override IEnumerator AutoFire()
    {
        // ȭ�鿡 ������ �������� �⺻ Enemy ������ ����
        yield return new WaitUntil(() => hasEnteredScreen);

        // ����ִ� ���� ���� �ݺ�
        while (gameObject.activeInHierarchy && !isDead)
        {
            // ���� ���� ���ݱ����� ��ü ������ (���� enemyFireDelay ���)
            yield return new WaitForSeconds(enemyFireDelay);

            // �÷��̾ ���ٸ� �߻� ������ �ǳʶ�
            if (player == null)
            {
                continue;
            }

            // --- 2���� �߻� ���� ���� ---
            for (int i = 0; i < burstCount; i++)
            {
                Vector3 fireDirection = (player.position - bulletSpawnPoint.position).normalized;
                if (fireDirection != Vector3.zero)
                {
                    Quaternion fireRotation = Quaternion.LookRotation(Vector3.forward, -fireDirection);
                    ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, bulletSpawnPoint.position, fireRotation);
                }

                // �� �Ѿ� ������ ª�� ���ݸ�ŭ ���
                yield return new WaitForSeconds(burstInterval);
            }
            // --- 2���� �߻� ���� �� ---
        }
    }
}