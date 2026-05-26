using TMPro;
using UnityEngine;

class HUDController : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text ammoText;

    void OnEnable() {
        ScoreManager.OnScoreChanged += UpdateScore;
        ScoreManager.OnKillsChanged += UpdateKills;
        GameManager.OnTimeChanged += UpdateTime;
        PlayerController.OnAmmoChanged += UpdateAmmo;
    }

    void Start() {
        UpdateScore(0);
        UpdateKills(0);
        UpdateTime(0);
    }

    void OnDisable() {
        ScoreManager.OnScoreChanged -= UpdateScore;
        ScoreManager.OnKillsChanged -= UpdateKills;
        GameManager.OnTimeChanged -= UpdateTime;
    }

    private void UpdateScore(int v) => scoreText.text = $"Score: {v}";
    private void UpdateKills(int v) => killsText.text = $"Kills: {v}";
    private void UpdateAmmo(int c, int m) => ammoText.text = $"Ammo: {c}/{m}";
    private void UpdateTime(float t) {
        int m = (int)(t / 60);
        int s = (int)(t % 60);
        timeText.text = $"Time: {m:00}:{s:00}";
    }
}