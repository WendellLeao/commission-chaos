using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace _Project
{
    internal sealed class PickupItem : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider col;

        private readonly SyncVar<bool> _isHeld = new();

        public bool IsHeld => _isHeld.Value;

        public void PickUp(NetworkBehaviour holder, Vector3 localPosition, Quaternion localRotation)
        {
            if (!IsServerInitialized || _isHeld.Value)
            {
                return;
            }

            NetworkObject.SetParent(holder);
            transform.SetLocalPositionAndRotation(localPosition, localRotation);
            _isHeld.Value = true;
        }

        public void Drop()
        {
            if (!IsServerInitialized || !_isHeld.Value)
            {
                return;
            }

            NetworkObject.UnsetParent();
            _isHeld.Value = false;
        }

        private void Awake()
        {
            _isHeld.OnChange += OnIsHeldChange;
        }

        private void OnDestroy()
        {
            _isHeld.OnChange -= OnIsHeldChange;
        }

        private void OnIsHeldChange(bool prev, bool next, bool asServer)
        {
            rb.isKinematic = next;
            col.enabled = !next;
        }
    }
}
