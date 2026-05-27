using System;
using UnityEngine;

class Rescuable : MonoBehaviour {
    [SerializeField] private int scoreReward = 500;
    [SerializeField] private GameObject rescueVFX;

    public static event Action OnRescued;
    public static event Action<Transform> OnSpawned;
    public static event Action OnDespawned;

    private bool _rescued = false;

    void Start() {
        OnSpawned?.Invoke(transform);
    }

    void OnTriggerEnter(Collider other) {
        if (_rescued) return;
        if (!other.CompareTag("Player")) return;

        _rescued = true;
        ScoreManager.Instance.AddScoreOnly(scoreReward);

        if (rescueVFX != null)
            Instantiate(rescueVFX, transform.position, Quaternion.identity);

        OnRescued?.Invoke();
        Destroy(gameObject);
    }

    void OnDestroy() {
        OnDespawned?.Invoke();
    }
}