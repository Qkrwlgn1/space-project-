using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI blinkingText;

    public float blinkSpeed = 0.5f;

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    public IEnumerator BlinkText()
    {
        while (true)
        {
            blinkingText.enabled = !blinkingText.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkSpeed); // Wait for the specified blink speed
        }
    }
}
