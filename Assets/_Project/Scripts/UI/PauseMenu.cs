using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class PauseMenu : MonoBehaviour {
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool _isPaused;

    void Awake() {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void OnEnable() {
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    void OnDisable() {
        if (resumeButton != null) resumeButton.onClick.RemoveListener(Resume);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    void Start() {
        if (AudioManager.Instance != null) {
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SfxVolume);
        }
    }

    void Update() {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (_isPaused) Resume();
            else Pause();
        }
    }

    private void Pause() {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.State.Running) return;
        _isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    private void Resume() {
        _isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void GoToMainMenu() {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioManager.Instance?.StopMusic();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnMusicChanged(float v) {
        AudioManager.Instance?.SetMusicVolume(v);
    }

    private void OnSfxChanged(float v) {
        AudioManager.Instance?.SetSfxVolume(v);
    }
}
