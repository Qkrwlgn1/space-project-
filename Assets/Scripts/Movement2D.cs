using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 0.0f;
    [SerializeField]
    public float minSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;


    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }


    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }
}
