using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;
using WendellLeao.Events;
using WendellLeao.ServiceLocator;

namespace Chaos.Gameplay.Timer
{
    public sealed class GameTimer : NetworkBehaviour
    {
        public event Action<float> OnTimeChanged;

        public const float MatchDuration = 120f;

        private readonly SyncVar<float> _remainingTime = new(MatchDuration);

        private bool _timeUpInvoked;
        private IEventService _eventService;

        public static GameTimer Instance { get; private set; }

        public float RemainingTime => _remainingTime.Value;

        private void Awake()
        {
            Instance = this;
            _eventService = Locator.Get<IEventService>();
            _remainingTime.OnChange += OnRemainingTimeChange;
        }

        private void OnDestroy()
        {
            _remainingTime.OnChange -= OnRemainingTimeChange;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (!IsServerInitialized || _remainingTime.Value <= 0f)
            {
                return;
            }

            _remainingTime.Value = Mathf.Max(0f, _remainingTime.Value - Time.deltaTime);
        }

        private void OnRemainingTimeChange(float prev, float next, bool asServer)
        {
            OnTimeChanged?.Invoke(next);

            if (Keyboard.current.backspaceKey.wasPressedThisFrame || !_timeUpInvoked && next <= 0f)
            {
                _timeUpInvoked = true;
                _eventService.DispatchEvent(new OnTimeUpEvent());
            }
        }
    }
}
