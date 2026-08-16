using System;
using _Project.Scripts.Networking;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace _Project
{
    internal sealed class ScoreManager : NetworkBehaviour
    {
        public event Action<int> OnScoreChanged;

        public const int WinScore = 500;
        public const int PointsPerItem = 100;

        private readonly SyncVar<int> _totalScore = new();

        public static ScoreManager Instance { get; private set; }

        public int TotalScore => _totalScore.Value;

        public void AddScore(int amount)
        {
            if (!IsServerInitialized)
            {
                return;
            }

            _totalScore.Value += amount;
        }

        private void Awake()
        {
            Instance = this;
            _totalScore.OnChange += OnTotalScoreChange;
        }

        private void Start()
        {
            StartCoroutine(BackendHandler.GetScoresRoutine());
        }

        private void OnDestroy()
        {
            _totalScore.OnChange -= OnTotalScoreChange;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnTotalScoreChange(int prev, int next, bool asServer)
        {
            OnScoreChanged?.Invoke(next);
        }
    }
}
