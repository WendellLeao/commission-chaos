using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project
{
    internal sealed class MyPlayerColorChanger : NetworkBehaviour
    {
        [SerializeField] private Renderer rend;
        [SerializeField] private Color nextColor;
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                MyPlayerIdProvider playerId = GetComponent<MyPlayerIdProvider>();
                ChangeColorServer(playerId, nextColor);
            }
        }

        [ServerRpc]
        private void ChangeColorServer(MyPlayerIdProvider playerId, Color color)
        {
            ChangeColorObserver(playerId, color);
        }

        [ObserversRpc]
        private void ChangeColorObserver(MyPlayerIdProvider playerId, Color color)
        {
            MyPlayerIdProvider id = GetComponent<MyPlayerIdProvider>();

            if (id != playerId)
            {
                return;
            }

            rend.material.color = color;
        }
    }
}
