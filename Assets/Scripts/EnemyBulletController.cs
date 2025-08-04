using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public float enemyBulletSpeed;
    public float enemyBulletDamage;


    void Update()
    {
        transform.position += -transform.up * enemyBulletSpeed * Time.deltaTime;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(enemyBulletDamage);
            }
            gameObject.SetActive(false);
        }
    }


    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}