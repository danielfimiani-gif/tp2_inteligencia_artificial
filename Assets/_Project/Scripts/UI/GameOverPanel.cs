using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class GameOverPanel : MonoBehaviour {
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button restartButton;

    void Awake() {
        panelRoot.SetActive(false);
    }

    void OnEnable() {
        GameManager.OnGameOver += Show;
        restartButton.onClick.AddListener(OnRestartClicked);
    }

    void OnDisable() {
        GameManager.OnGameOver -= Show;
        restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    void Show() {
        panelRoot.SetActive(true);

        int score = ScoreManager.Instance.Score;
        int kills = ScoreManager.Instance.Kills;
        float t = GameManager.Instance.TimeSurvived;

        int m = (int)(t / 60);
        int s = (int)(t % 60);

        scoreText.text = $"Score: {score}";
        killsText.text = $"Kills: {kills}";
        timeText.text = $"Time: {m:00}:{s:00}";
    }

    private void OnRestartClicked() {
        GameManager.Instance.Restart();
    }
}