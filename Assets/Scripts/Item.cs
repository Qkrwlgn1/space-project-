using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public float fallSpeed;

    
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            PlayerController player = collision.GetComponent<PlayerController>();
            Debug.Log("Item collected by player");

            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
