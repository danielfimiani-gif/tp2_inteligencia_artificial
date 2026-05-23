using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class MainMenu : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string gameScene;

    void Awake() {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton.onClick.AddListener(OptionsButtonClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);
    }

    void Start() {
#if UNITY_WEBGL
    exitButton.gameObject.SetActive(false);
#endif
    }

    void OnDestroy() {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        optionsButton.onClick.RemoveListener(OptionsButtonClicked);
        exitButton.onClick.RemoveListener(ExitButtonClicked);
    }

    private void OnPlayButtonClicked() {
        if (!string.IsNullOrEmpty(gameScene)) SceneManager.LoadScene(gameScene);
    }

    private void OptionsButtonClicked() {
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    private void ExitButtonClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit()
#endif
    }
}