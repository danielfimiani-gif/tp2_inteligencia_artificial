using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PooledBullet : MonoBehaviour {
    [SerializeField] private float damage = 20;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private TrailRenderer trailRenderer;

    private ObjectPool<GameObject> _originPool;
    private float _lifeTimer;

    private void OnEnable() {
        _lifeTimer = lifetime;
        if (trailRenderer != null) {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
            StartCoroutine(EnablTrailNextFrame());
        }
    }

    private void Update() {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f) {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter(Collider other) {
        var damageable = other.GetComponentInParent<IDamageable>();
        damageable?.ReceiveDamage(damage);
        ReleaseToPool();
    }

    void OnDisable() {
        if (trailRenderer != null) {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }
    }

    public void SetupPool(ObjectPool<GameObject> pool) {
        _originPool = pool;
    }

    private void ReleaseToPool() {
        if (_originPool != null && gameObject.activeSelf) {
            _originPool.Release(gameObject);
        }
    }

    private IEnumerator EnablTrailNextFrame() {
        yield return null;
        if (trailRenderer != null) trailRenderer.emitting = true;
    }
}