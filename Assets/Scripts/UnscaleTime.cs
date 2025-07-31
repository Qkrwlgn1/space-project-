using UnityEngine;

public class UnscaleTime : MonoBehaviour
{
    float timer;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
    }
}
