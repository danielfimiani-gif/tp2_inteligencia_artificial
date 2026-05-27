using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class WaveManager : MonoBehaviour {
    public static WaveManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject fastPrefab;
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject rangedPrefab;

    [Header("NPC Rescuable")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] npcSpawnPoints;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaveData[] waves;

    [Header("Endless")]
    [SerializeField] private bool endlessLoop = true;

    [Header("Drops")]
    [SerializeField] private GameObject ammoPickupPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float magDropChance = 0.2f;

    [Header("Boss Mode")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private int bossModeWaveCount = 5;

    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCleared;

    private int _aliveCount;
    private GameObject _currentNPC;

    public GameObject CurrentNpc => _currentNPC;

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
        RangedDieSMB.OnRangedZombieDied += HandleRangedDied;
        BossBrain.OnBossDied += HandleBossDied;
    }

    void OnDisable() {
        DieSMB.OnZombieDied -= HandleZombieDied;
        RangedDieSMB.OnRangedZombieDied -= HandleRangedDied;
        BossBrain.OnBossDied -= HandleBossDied;
    }

    private bool _bossDefeated;
    private void HandleBossDied(BossBrain _) => _bossDefeated = true;

    void Start() {
        AudioManager.Instance?.PlayMusic("GameplayMusic");

        if (GameModeSelection.Selected == GameMode.Boss) {
            StartCoroutine(RunBossMode());
        } else {
            StartCoroutine(RunWaves());
        }
    }

    IEnumerator RunBossMode() {
        int n = Mathf.Min(bossModeWaveCount, waves.Length);
        for (int i = 0; i < n; i++) {
            yield return new WaitForSeconds(waves[i].breakBefore);
            SpawnNPC();
            OnWaveStarted?.Invoke(i + 1);
            AudioManager.Instance?.PlaySFX("WaveStart");
            yield return SpawnWave(waves[i]);
            yield return new WaitUntil(() => _aliveCount == 0);
            OnWaveCleared?.Invoke(i + 1);
            CleanupNPC();
        }

        yield return new WaitForSeconds(3f);
        SpawnBoss();
        yield return new WaitUntil(() => _bossDefeated);
        yield return new WaitForSeconds(2f);
        GameManager.Instance?.Win();
    }

    private void SpawnBoss() {
        if (bossPrefab == null) {
            Debug.LogWarning("[WaveManager] bossPrefab no asignado.");
            return;
        }
        Vector3 pos = bossSpawnPoint != null ? bossSpawnPoint.position : Vector3.zero;
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
            pos = hit.position;
        }
        Instantiate(bossPrefab, pos, Quaternion.identity, transform);
    }

    IEnumerator RunWaves() {
        for (int i = 0; i < waves.Length; i++) {
            yield return new WaitForSeconds(waves[i].breakBefore);
            SpawnNPC();
            OnWaveStarted?.Invoke(i + 1);
            AudioManager.Instance?.PlaySFX("WaveStart");
            yield return SpawnWave(waves[i]);
            yield return new WaitUntil(() => _aliveCount == 0);
            OnWaveCleared?.Invoke(i + 1);
            CleanupNPC();
        }

        if (!endlessLoop || waves.Length == 0) yield break;

        WaveData last = waves[^1];
        while (true) {
            yield return new WaitForSeconds(last.breakBefore);
            yield return SpawnWave(last);
            yield return new WaitUntil(() => _aliveCount == 0);
            last.MeleeCount++;
        }
    }

    IEnumerator SpawnWave(WaveData wave) {
        var queue = new List<GameObject>();
        for (int i = 0; i < wave.MeleeCount; i++) queue.Add(meleePrefab);
        for (int i = 0; i < wave.FastCount; i++) queue.Add(fastPrefab);
        for (int i = 0; i < wave.TankCount; i++) queue.Add(tankPrefab);
        for (int i = 0; i < wave.RangedCount; i++) queue.Add(rangedPrefab);
        Shuffle(queue);

        foreach (var prefab in queue) {
            SpawnOne(prefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void SpawnOne(GameObject prefab) {
        if (prefab == null || spawnPoints.Length == 0) return;
        Transform sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Vector3 pos = sp.position;
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 5f, NavMesh.AllAreas)) {
            pos = hit.position;
        }
        Instantiate(prefab, pos, Quaternion.identity, sp);
        _aliveCount++;
    }

    private void HandleZombieDied(ZombieBrain z) {
        _aliveCount--;
        TryDropMag(z.transform.position);
    }
    private void HandleRangedDied(RangedBrain z) {
        _aliveCount--;
        TryDropMag(z.transform.position);
    }

    private void Shuffle<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void TryDropMag(Vector3 worldPos) {
        if (ammoPickupPrefab == null) return;
        if (UnityEngine.Random.value > magDropChance) return;
        Instantiate(ammoPickupPrefab, worldPos + Vector3.up * 0.5f, Quaternion.identity, transform);
    }

    private void SpawnNPC() {
        if (npcPrefab == null || npcSpawnPoints.Length == 0) return;

        Transform sp = npcSpawnPoints[UnityEngine.Random.Range(0, npcSpawnPoints.Length)];
        _currentNPC = Instantiate(npcPrefab, sp.position, Quaternion.identity, sp);
    }

    private void CleanupNPC() {
        if (_currentNPC != null) {
            Destroy(_currentNPC);
            _currentNPC = null;
        }
    }
}
