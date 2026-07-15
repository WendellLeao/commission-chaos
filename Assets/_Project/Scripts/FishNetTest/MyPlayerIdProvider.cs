using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace _Project
{
    internal sealed class MyPlayerIdProvider : NetworkBehaviour
    {
        private readonly SyncVar<string> _playerId = new();

        public string PlayerId => _playerId.Value;
        
        // Debug
        public string playerIdDebug;
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;
            
            if (!IsOwner)
            {
                return;
            }
            
            string playerId = BuildPlayerId();
            
            SetPlayerIdServer(playerId);
        }
        
        private void Awake()
        {
            _playerId.OnChange += OnPlayerIdChange;
        }

        private void OnDestroy()
        {
            _playerId.OnChange -= OnPlayerIdChange;
        }

        private void OnPlayerIdChange(string prev, string next, bool asServer)
        {
            playerIdDebug = next;
        }
        
        [ServerRpc]
        private void SetPlayerIdServer(string playerId)
        {
            _playerId.Value = playerId;
        }

        private string BuildPlayerId()
        {
            if (IsHostInitialized)
            {
                return "host_player";
            }

            if (IsClientOnlyInitialized)
            {
                return "client_player";
            }

            return "";
        }
    }
}
