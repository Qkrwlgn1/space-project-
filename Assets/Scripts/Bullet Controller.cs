using UnityEngine;
public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    public float playerBulletDamage;
    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
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
}
