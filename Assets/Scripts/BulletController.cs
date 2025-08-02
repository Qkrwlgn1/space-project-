using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
public class BulletController : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }
    public float bulletSpeed;
    public PlayerController playerCon;

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
            
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerCon.playerBulletDamage);
            }
            Pool.Release(this.gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Pool.Release(this.gameObject);
    }

}
