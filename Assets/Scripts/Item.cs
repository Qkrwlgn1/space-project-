using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public float fallSpeed;

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
            return;

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
