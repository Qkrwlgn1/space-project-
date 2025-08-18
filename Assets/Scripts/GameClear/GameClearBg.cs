using UnityEngine;


public class MoveBackgroundWin : MonoBehaviour
{
    public Renderer targetRenderer;
    public float speed = 0.5f;
    public Material mat;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        float move = Time.deltaTime * speed;
        mat.mainTextureOffset += Vector2.up * move;
    }
}
