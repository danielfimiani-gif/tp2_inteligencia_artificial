using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour {
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;
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
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private GameObject CreateBullet() {
        GameObject bullet = Instantiate(bulletPrefab, transform);
        bullet.GetComponent<PooledBullet>().SetupPool(pool);
        return bullet;
    }

    private void OnGetBullet(GameObject bullet) => bullet.SetActive(true);
    private void OnReleaseBullet(GameObject bullet) => bullet.SetActive(false);
    private void OnDestroyBullet(GameObject bullet) => Destroy(bullet);

    public GameObject GetBullet(Vector3 position, Quaternion rotation) {
        GameObject bullet = pool.Get();
        bullet.transform.SetPositionAndRotation(position, rotation);
        return bullet;
    }
}