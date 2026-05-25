using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectilePool : MonoBehaviour {
    public static EnemyProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<GameObject> pool;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        pool = new ObjectPool<GameObject>(
            createFunc: CreateProjectile,
            actionOnRelease: p => p.SetActive(false),
            actionOnDestroy: p => Destroy(p),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private GameObject CreateProjectile() {
        GameObject p = Instantiate(projectilePrefab, transform);
        p.GetComponent<PooledBlob>().SetupPool(pool);
        return p;
    }

    public void Launch(Vector3 spawnPos, Vector3 targetPos, float flightTime = 1.0f) {
        GameObject p = pool.Get();
        p.transform.position = spawnPos;
        p.SetActive(true);
        p.GetComponent<PooledBlob>().Launch(targetPos, flightTime);
    }
}