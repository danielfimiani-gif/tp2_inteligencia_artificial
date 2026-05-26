using System;
using UnityEngine;

class ScoreManager : MonoBehaviour {
    [SerializeField] private int scoreZombieMelee = 10;
    [SerializeField] private int scoreZombieRanged = 10;

    public static ScoreManager Instance { get; private set; }

    private int _score = 0;
    private int _kills = 0;

    public int Score {
        get => _score;
        set {
            _score = value;
            OnScoreChanged?.Invoke(_score);
        }
    }
    public int Kills {
        get => _kills;
        set {
            _kills = value;
            OnKillsChanged?.Invoke(_kills);
        }
    }

    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnKillsChanged;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable() {
        DieSMB.OnZombieDied += HandleZombieDied;
        RangedDieSMB.OnRangedZombieDied += HandleRangedZombieDied;
    }

    void OnDisable() {
        DieSMB.OnZombieDied -= HandleZombieDied;
        RangedDieSMB.OnRangedZombieDied -= HandleRangedZombieDied;
    }

    public void AddKill(int scoreValue) {
        Kills++;
        Score += scoreValue;
    }

    public void Reset() {
        Score = 0;
        Kills = 0;
    }

    public void HandleZombieDied(ZombieBrain _) => AddKill(scoreZombieMelee);
    public void HandleRangedZombieDied(RangedBrain _) => AddKill(scoreZombieRanged);
}