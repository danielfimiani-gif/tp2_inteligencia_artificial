using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
class BossBrain : MonoBehaviour, IDamageable {
    [Header("Stats")]
    [SerializeField] private float maxHealth = 800f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private HealthBar healthBar;

    [Header("Phases")]
    [SerializeField] private float phase2Threshold = 0.66f;
    [SerializeField] private float phase3Threshold = 0.33f;
    [SerializeField] private float speedPhase1 = 2.5f;
    [SerializeField] private float speedPhase2 = 3.0f;
    [SerializeField] private float speedPhase3 = 4.5f;
    [SerializeField] private float damagePhase3Multiplier = 1.5f;

    private static readonly int HashPhase = Animator.StringToHash("Phase");
    private static readonly int HashDistance = Animator.StringToHash("DistanceToPlayer");
    private static readonly int HashDie = Animator.StringToHash("Die");

    public static event Action<BossBrain> OnBossDied;
    public static event Action<int> OnBossPhaseChanged;

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Target;
    public float AttackDamage => attackDamage;
    public int CurrentPhase { get; private set; } = 1;
    public float CurrentHealth { get; private set; }

    private float _baseDamage;

    void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        if (Target == null) Target =
            GameObject.FindGameObjectWithTag("Player")?.transform;
        CurrentHealth = maxHealth;
        _baseDamage = attackDamage;
        ApplyPhase(1);
        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);
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
            Vector3 spawnPos = transform.position + Vector3.up * 1.8f;
            Instantiate(bloodParticles, spawnPos, Quaternion.identity);
        }

        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0) {
            Animator.SetTrigger(HashDie);
            OnBossDied?.Invoke(this);
            return;
        }

        float ratio = CurrentHealth / maxHealth;
        int desiredPhase = CurrentPhase;
        if (ratio < phase3Threshold) desiredPhase = 3;
        else if (ratio < phase2Threshold) desiredPhase = 2;

        if (desiredPhase != CurrentPhase) {
            ApplyPhase(desiredPhase);
        }
    }

    private void ApplyPhase(int phase) {
        CurrentPhase = phase;
        Animator.SetInteger(HashPhase, phase);

        switch (phase) {
            case 1:
                Agent.speed = speedPhase1;
                attackDamage = _baseDamage;
                break;
            case 2:
                Agent.speed = speedPhase2;
                attackDamage = _baseDamage;
                break;
            case 3:
                Agent.speed = speedPhase3;
                attackDamage = _baseDamage * damagePhase3Multiplier;
                break;
        }

        OnBossPhaseChanged?.Invoke(phase);
    }
}
