using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
class ZombieBrain : MonoBehaviour, IDamageable {
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private GameObject bloodParticles;

    private static readonly int HashDistance = Animator.StringToHash("DistanceToPlayer");
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashDie = Animator.StringToHash("Die");

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Target;
    public float AttackDamage => attackDamage;

    public float CurrentHealth { get; private set; }

    void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        if (Target == null) Target =
            GameObject.FindGameObjectWithTag("Player")?.transform;
        CurrentHealth = maxHealth;
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
    }
}