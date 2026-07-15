using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project
{
    internal sealed class GameplayHudController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Label _scoreLabel;
        private VisualElement _winPanel;
        private ScoreManager _scoreManager;
        private bool _winPanelShown;

        private void OnEnable()
        {
            VisualElement root = uiDocument.rootVisualElement;

            _scoreLabel = root.Q<Label>("score-label");
            _winPanel = root.Q<VisualElement>("win-panel");

            _winPanelShown = false;
            _winPanel.style.display = DisplayStyle.None;
            _scoreLabel.text = BuildScoreText(0);

            StartCoroutine(WaitForScoreManager());
        }

        private void OnDisable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= OnScoreChanged;
            }

            StopAllCoroutines();
        }

        private IEnumerator WaitForScoreManager()
        {
            while (ScoreManager.Instance == null)
            {
                yield return null;
            }

            _scoreManager = ScoreManager.Instance;
            _scoreManager.OnScoreChanged += OnScoreChanged;
            OnScoreChanged(_scoreManager.TotalScore);
        }

        private void OnScoreChanged(int score)
        {
            _scoreLabel.text = BuildScoreText(score);

            if (!_winPanelShown && score >= ScoreManager.WinScore)
            {
                _winPanelShown = true;
                _winPanel.style.display = DisplayStyle.Flex;
            }
        }

        private string BuildScoreText(int score)
        {
            return $"Pontos: {score}";
        }
    }
}
