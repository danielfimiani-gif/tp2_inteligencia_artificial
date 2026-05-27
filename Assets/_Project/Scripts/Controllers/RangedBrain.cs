using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
class RangedBrain : MonoBehaviour, IDamageable {
    [Header("Stats")]
    [SerializeField] private float maxHealth = 80f;

    [Header("Ranged")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject bloodParticles;

    [SerializeField] private HealthBar healthBar;

    [Header("Enraged")]
    [SerializeField] private float enragedHealthThreshold = 0.3f;
    [SerializeField] private float enragedSpeedMultiplier = 1.5f;
    [SerializeField] private Outline enragedOutline;
    [SerializeField] private Color enragedOutlineColor = new Color(1f, 0.5f, 0f);

    private static readonly int HashDistance = Animator.StringToHash("DistanceToPlayer");
    private static readonly int HashDie = Animator.StringToHash("Die");

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Target;
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    public float CurrentHealth { get; private set; }

    private bool _enraged;
    private float _baseSpeed;
    private Color _baseOutlineColor;

    void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Target = GameObject.FindGameObjectWithTag("Player")?.transform;
        CurrentHealth = maxHealth;
        _baseSpeed = Agent.speed;
        if (enragedOutline != null) _baseOutlineColor = enragedOutline.OutlineColor;
        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);
    }

    void OnDestroy() {
        if (enragedOutline != null) enragedOutline.OutlineColor = _baseOutlineColor;
    }

    void Start() {
        if (!Agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            Agent.Warp(hit.position);
    }

    void Update() {
        if (Target == null) return;
        Animator.SetFloat(HashDistance, Vector3.Distance(transform.position, Target.position));
    }

    public void ReceiveDamage(float damageAmount) {
        if (CurrentHealth <= 0) return;
        CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);
        if (bloodParticles != null)
            Instantiate(bloodParticles, transform.position + Vector3.up * 1.2f, Quaternion.identity);
        if (CurrentHealth <= 0)
            Animator.SetTrigger(HashDie);

        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);

        if (!_enraged && CurrentHealth > 0 && CurrentHealth / maxHealth < enragedHealthThreshold) {
            EnterEnraged();
        }
    }

    private void EnterEnraged() {
        _enraged = true;
        Agent.speed = _baseSpeed * enragedSpeedMultiplier;
        if (enragedOutline != null) enragedOutline.OutlineColor = enragedOutlineColor;
    }
}