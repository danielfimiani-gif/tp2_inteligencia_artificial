using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class PooledBlob : MonoBehaviour {
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private TrailRenderer trailRenderer;

    private Rigidbody _rb;
    private ObjectPool<GameObject> _originPool;
    private float _lifeTimer;

    void Awake() { _rb = GetComponent<Rigidbody>(); }

    void OnEnable() {
        _lifeTimer = lifetime;
        if (trailRenderer != null) {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
            StartCoroutine(EnableTrailNextFrame());
        }
    }

    IEnumerator EnableTrailNextFrame() {
        yield return null;
        if (trailRenderer != null) trailRenderer.emitting = true;
    }

    void OnDisable() {
        if (_rb != null) {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        if (trailRenderer != null) {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }
    }

    public void SetupPool(ObjectPool<GameObject> pool) => _originPool = pool;

    public void Launch(Vector3 targetPosition, float flightTime) {
        Vector3 displacement = targetPosition - transform.position;
        float g = Mathf.Abs(Physics.gravity.y);

        Vector3 vXZ = new Vector3(displacement.x, 0, displacement.z) / flightTime;
        float vY = (displacement.y + 0.5f * g * flightTime * flightTime) / flightTime;

        _rb.linearVelocity = vXZ + Vector3.up * vY;
    }

    void Update() {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f) ReleaseToPool();
    }

    void OnTriggerEnter(Collider other) {
        var damageable = other.GetComponentInParent<IDamageable>();
        damageable?.ReceiveDamage(damage);
        ReleaseToPool();
    }

    void ReleaseToPool() {
        if (_originPool != null && gameObject.activeSelf)
            _originPool.Release(gameObject);
    }
}