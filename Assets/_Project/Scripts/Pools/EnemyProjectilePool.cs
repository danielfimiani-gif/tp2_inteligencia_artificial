using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectilePool : MonoBehaviour {
    public static EnemyProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 50;

    private ObjectPool<GameObject> pool;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        pool = new ObjectPool<GameObject>(
            createFunc: CreateBullet,
            actionOnRelease: OnReleaseProjectile,
            actionOnDestroy: OnDestroyProjectile,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private GameObject CreateBullet() {
        GameObject bullet = Instantiate(projectilePrefab, transform);
        bullet.GetComponentInChildren<PooledBullet>().SetupPool(pool);
        return bullet;
    }

    private void OnReleaseProjectile(GameObject bullet) => bullet.SetActive(false);
    private void OnDestroyProjectile(GameObject bullet) => Destroy(bullet);

    public GameObject GetProjectile(Vector3 position, Quaternion rotation) {
        GameObject bullet = pool.Get();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.SetActive(true);
        return bullet;
    }
}