using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class MainMenu : MonoBehaviour {
    [SerializeField] private Button endlessButton;
    [SerializeField] private Button bossButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string gameScene;

    void Awake() {
        endlessButton.onClick.AddListener(OnEndlessClicked);
        bossButton.onClick.AddListener(OnBossClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);
    }

    void Start() {
#if UNITY_WEBGL
        exitButton.gameObject.SetActive(false);
#endif
    }

    void OnDestroy() {
        endlessButton.onClick.RemoveListener(OnEndlessClicked);
        bossButton.onClick.RemoveListener(OnBossClicked);
        exitButton.onClick.RemoveListener(ExitButtonClicked);
    }

    private void OnEndlessClicked() => StartGame(GameMode.Endless);
    private void OnBossClicked() => StartGame(GameMode.Boss);

    private void StartGame(GameMode mode) {
        GameModeSelection.Selected = mode;
        if (!string.IsNullOrEmpty(gameScene)) SceneManager.LoadScene(gameScene);
    }

    private void ExitButtonClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
