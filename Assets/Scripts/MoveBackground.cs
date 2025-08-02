using UnityEngine;


public class MoveBackground : MonoBehaviour
{
    public new Renderer renderer;
    public float speed = 0.5f;

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;
            
        float move = Time.deltaTime * speed;
        renderer.material.mainTextureOffset += Vector2.up * move;
    }
}
