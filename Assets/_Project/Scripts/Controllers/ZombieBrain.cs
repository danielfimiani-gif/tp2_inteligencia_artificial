using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
class ZombieBrain : MonoBehaviour, IDamageable {
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private HealthBar healthBar;

    [Header("Enraged")]
    [SerializeField] private float enragedHealthThreshold = 0.3f;
    [SerializeField] private float enragedSpeedMultiplier = 1.5f;
    [SerializeField] private float enragedDamageMultiplier = 1.5f;
    [SerializeField] private Outline enragedOutline;
    [SerializeField] private Color enragedOutlineColor = new Color(1f, 0.5f, 0f);

    private static readonly int HashDistance = Animator.StringToHash("DistanceToPlayer");
    private static readonly int HashDie = Animator.StringToHash("Die");

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Target;
    public float AttackDamage => attackDamage;

    public float CurrentHealth { get; private set; }

    private bool _enraged;
    private float _baseSpeed;
    private float _baseDamage;
    private Color _baseOutlineColor;

    void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        if (Target == null) Target =
            GameObject.FindGameObjectWithTag("Player")?.transform;
        CurrentHealth = maxHealth;
        _baseSpeed = Agent.speed;
        _baseDamage = attackDamage;
        if (enragedOutline != null) _baseOutlineColor = enragedOutline.OutlineColor;
        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);

    }

    void OnDestroy() {
        if (enragedOutline != null) enragedOutline.OutlineColor = _baseOutlineColor;
    }

    void Update() {
        if (Target == null) return;
        var dist = Vector3.Distance(transform.position, Target.position);
        Animator.SetFloat(HashDistance, dist);
    }

    public void ReceiveDamage(float damageAmount) {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);

        if (bloodParticles != null) {
            Vector3 spawnPos = transform.position + Vector3.up * 1.2f;
            Instantiate(bloodParticles, spawnPos, Quaternion.identity);
        }

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
        attackDamage = _baseDamage * enragedDamageMultiplier;
        if (enragedOutline != null) enragedOutline.OutlineColor = enragedOutlineColor;
    }
}