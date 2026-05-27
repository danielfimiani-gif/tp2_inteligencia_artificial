using System;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public enum State {
        Running,
        GameOver,
        Victory
    }

    private float _timeSurvived = 0;
    private State _currentState = State.Running;

    public State CurrentState {
        get => _currentState;
        set {
            _currentState = value;
            if (_currentState == State.GameOver) OnGameOver?.Invoke();
            else if (_currentState == State.Victory) OnVictory?.Invoke();
        }
    }
    public float TimeSurvived {
        get => _timeSurvived;
        set {
            _timeSurvived = value;
            OnTimeChanged?.Invoke(_timeSurvived);
        }
    }

    public static event Action<float> OnTimeChanged;
    public static event Action OnGameOver;
    public static event Action OnVictory;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;

        }
    }

    void OnEnable() {
        PlayerController.OnPlayerDied += EndGame;
    }


    void Update() {
        if (CurrentState != State.Running) return;
        TimeSurvived += Time.deltaTime;
    }

    void OnDisable() {
        PlayerController.OnPlayerDied -= EndGame;
    }

    public void EndGame() {
        if (CurrentState != State.Running) return;
        CurrentState = State.GameOver;
        Time.timeScale = 0f;
    }

    public void Win() {
        if (CurrentState != State.Running) return;
        CurrentState = State.Victory;
        Time.timeScale = 0f;
    }

    public void Restart() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}