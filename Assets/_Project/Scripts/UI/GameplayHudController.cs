using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project
{
    internal sealed class GameplayHudController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Label _scoreLabel;
        private Label _timerLabel;
        private VisualElement _winPanel;
        private VisualElement _losePanel;
        private ScoreManager _scoreManager;
        private GameTimer _gameTimer;
        private bool _winPanelShown;
        private bool _losePanelShown;

        private void OnEnable()
        {
            VisualElement root = uiDocument.rootVisualElement;

            _scoreLabel = root.Q<Label>("score-label");
            _timerLabel = root.Q<Label>("timer-label");
            _winPanel = root.Q<VisualElement>("win-panel");
            _losePanel = root.Q<VisualElement>("lose-panel");

            _winPanelShown = false;
            _losePanelShown = false;
            _winPanel.style.display = DisplayStyle.None;
            _losePanel.style.display = DisplayStyle.None;
            _scoreLabel.text = BuildScoreText(0);
            _timerLabel.text = BuildTimerText(GameTimer.MatchDuration);

            StartCoroutine(WaitForScoreManager());
            StartCoroutine(WaitForGameTimer());
        }

        private void OnDisable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= OnScoreChanged;
            }

            if (_gameTimer != null)
            {
                _gameTimer.OnTimeChanged -= OnTimeChanged;
                _gameTimer.OnTimeUp -= OnTimeUp;
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

        private IEnumerator WaitForGameTimer()
        {
            while (GameTimer.Instance == null)
            {
                yield return null;
            }

            _gameTimer = GameTimer.Instance;
            _gameTimer.OnTimeChanged += OnTimeChanged;
            _gameTimer.OnTimeUp += OnTimeUp;
            OnTimeChanged(_gameTimer.RemainingTime);
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

        private void OnTimeChanged(float remainingTime)
        {
            _timerLabel.text = BuildTimerText(remainingTime);
        }

        private void OnTimeUp()
        {
            if (_winPanelShown || _losePanelShown)
            {
                return;
            }

            _losePanelShown = true;
            _losePanel.style.display = DisplayStyle.Flex;
        }

        private string BuildScoreText(int score)
        {
            return $"Score: {score}";
        }

        private string BuildTimerText(float remainingTime)
        {
            return $"Time: {Mathf.CeilToInt(remainingTime)}";
        }
    }
}
