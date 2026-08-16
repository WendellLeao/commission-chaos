using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace _Project
{
    internal sealed class MyCharacterIdProvider : NetworkBehaviour
    {
        private readonly SyncVar<string> _characterId = new();

        public string CharacterId => _characterId.Value;

        // Debug
        public string characterIdDebug;

        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;

            if (!IsOwner)
            {
                return;
            }

            string characterId = BuildCharacterId();

            SetCharacterIdServer(characterId);
        }

        private void Awake()
        {
            _characterId.OnChange += OnCharacterIdChange;
        }

        private void OnDestroy()
        {
            _characterId.OnChange -= OnCharacterIdChange;
        }

        private void OnCharacterIdChange(string prev, string next, bool asServer)
        {
            characterIdDebug = next;
        }

        [ServerRpc]
        private void SetCharacterIdServer(string characterId)
        {
            _characterId.Value = characterId;
        }

        private string BuildCharacterId()
        {
            if (IsHostInitialized)
            {
                return "host_character";
            }

            if (IsClientOnlyInitialized)
            {
                return "client_character";
            }

            return "";
        }
    }
}
