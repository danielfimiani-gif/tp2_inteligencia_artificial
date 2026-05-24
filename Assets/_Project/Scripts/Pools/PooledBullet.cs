using UnityEngine;
using UnityEngine.Pool;

public class PooledBullet : MonoBehaviour {
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    private ObjectPool<GameObject> originPool;
    private float lifeTimer;

    public void SetupPool(ObjectPool<GameObject> pool) {
        originPool = pool;
    }

    private void OnEnable() {
        lifeTimer = lifetime;
    }

    private void Update() {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter(Collider other) {
        ReleaseToPool();
    }

    private void ReleaseToPool() {
        if (originPool != null && gameObject.activeSelf) {
            originPool.Release(gameObject);
        }
    }
}