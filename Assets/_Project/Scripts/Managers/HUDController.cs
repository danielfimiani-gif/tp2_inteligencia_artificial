using System;
using TMPro;
using UnityEngine;

class HUDController : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text magsText;

    void OnEnable() {
        ScoreManager.OnScoreChanged += UpdateScore;
        ScoreManager.OnKillsChanged += UpdateKills;
        GameManager.OnTimeChanged += UpdateTime;
        PlayerController.OnAmmoChanged += UpdateAmmo;
        PlayerController.OnMagazinesChanged += UpdateMags;
    }

    void Start() {
        UpdateScore(0);
        UpdateKills(0);
        UpdateTime(0);

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) {
            UpdateAmmo(player.CurrentAmmo, player.CurrentAmmo);
            UpdateMags(player.CurrentMagazines);
        }
    }

    void OnDisable() {
        ScoreManager.OnScoreChanged -= UpdateScore;
        ScoreManager.OnKillsChanged -= UpdateKills;
        GameManager.OnTimeChanged -= UpdateTime;
        PlayerController.OnAmmoChanged -= UpdateAmmo;
        PlayerController.OnMagazinesChanged -= UpdateMags;
    }

    private void UpdateScore(int v) => scoreText.text = $"Score: {v}";
    private void UpdateKills(int v) => killsText.text = $"Kills: {v}";
    private void UpdateAmmo(int c, int m) => ammoText.text = $"Ammo: {c}/{m}";
    private void UpdateMags(int v) => magsText.text = $"Mags : {v}";
    private void UpdateTime(float t) {
        int m = (int)(t / 60);
        int s = (int)(t % 60);
        timeText.text = $"Time: {m:00}:{s:00}";
    }
}