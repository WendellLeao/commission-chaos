using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace _Project
{
    internal sealed class MyCharacterColorChanger : NetworkBehaviour
    {
        [SerializeField] private Renderer rend;
        [SerializeField] private Color[] colorPalette;

        private readonly SyncVar<Color> _color = new();

        public override void OnStartServer()
        {
            base.OnStartServer();

            _color.Value = colorPalette[Owner.ClientId % colorPalette.Length];
        }

        private void Awake()
        {
            _color.OnChange += OnColorChange;
        }

        private void OnDestroy()
        {
            _color.OnChange -= OnColorChange;
        }

        private void OnColorChange(Color prev, Color next, bool asServer)
        {
            rend.material.color = next;
        }
    }
}
