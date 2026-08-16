using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Chaos.UI.TitleScreen
{
    internal sealed class TitleScreenController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private string _gameplaySceneName = "SampleScene";

        private Button _playButton;
        private Button _quitButton;

        private void OnEnable()
        {
            VisualElement root = _uiDocument.rootVisualElement;

            _playButton = root.Q<Button>("play-button");
            _quitButton = root.Q<Button>("quit-button");

            _playButton.clicked += OnPlayClicked;
            _quitButton.clicked += OnQuitClicked;
        }

        private void OnDisable()
        {
            _playButton.clicked -= OnPlayClicked;
            _quitButton.clicked -= OnQuitClicked;
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(_gameplaySceneName);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
