using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    public float playerBulletDamage;


    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
            
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
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
            gameObject.SetActive(false);
        }
    }

}
