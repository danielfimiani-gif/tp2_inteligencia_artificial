using UnityEngine;

public class AmmoPickup : MonoBehaviour {
    [SerializeField] private float bobAmplitude = 0.2f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotateSpeed = 60f;

    private Vector3 _basePos;
    private bool _picked;

    void Start() {
        _basePos = transform.position;
    }

    void Update() {
        float y = _basePos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = new Vector3(_basePos.x, y, _basePos.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (_picked) return;
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _picked = true;
        player.AddMagazine();
        AudioManager.Instance?.PlaySFX("AmmoPickup");
        Destroy(gameObject);
    }
}