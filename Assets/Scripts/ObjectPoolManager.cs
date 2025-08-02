using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public int defaultCapacity = 5;
    public int maxPoolSize = 15;

    public GameObject bulletPrefab;

    public IObjectPool<GameObject> Pool { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        Init();
    }

    void Init()
    {
        Pool = new ObjectPool<GameObject>(CreatePoolItem, OnTakeFromPool, OnReturnToPool,
        OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

        for (int i = 0; i < defaultCapacity; i++)
        {
            BulletController bullet = CreatePoolItem().GetComponent<BulletController>();
            bullet.Pool.Release(bullet.gameObject);
        }
    }

    private GameObject CreatePoolItem() {

        
        GameObject poolG = Instantiate(bulletPrefab);
        poolG.GetComponent<BulletController>().Pool = this.Pool;
        return poolG;
    }

    private void OnTakeFromPool(GameObject poolG)
    {
        poolG.SetActive(true);
    }

    private void OnReturnToPool(GameObject poolG)
    {
        poolG.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolG)
    {
        Destroy(poolG);
    }
}
