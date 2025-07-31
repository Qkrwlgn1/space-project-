using UnityEngine;
using UnityEngine.Pool;
public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    public int playerBulletDamage;

    private IObjectPool<BulletController> _ManagePool;
    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void SetManagePool(IObjectPool<BulletController> pool)
    {
        _ManagePool = pool;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerBulletDamage);
            }
            Destroy(gameObject);
        }
    }
    public void DestroyBullet()
    {
        _ManagePool.Release(this);
    }
}
