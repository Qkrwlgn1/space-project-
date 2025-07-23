using UnityEngine;


public class MoveBackground : MonoBehaviour
{
    public Renderer renderer;
    public float speed = 0.5f;


    private void Update()
    {
        float move = Time.deltaTime * speed;
        renderer.material.mainTextureOffset += Vector2.up * move;
    }
}
