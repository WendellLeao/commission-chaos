using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project
{
    internal sealed class PlayerPickup : NetworkBehaviour
    {
        [SerializeField] private Transform handSocket;
        [SerializeField] private float pickupRadius = 1.5f;
        [SerializeField] private LayerMask pickupLayer;

        private PickupItem _heldItem;

        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;
        }

        public void ClearHeldItemServer()
        {
            if (!IsServerInitialized)
            {
                return;
            }

            TargetSetHeldItem(Owner, null);
        }

        private void Update()
        {
            if (!ReadInteractPressed())
            {
                return;
            }

            if (_heldItem != null)
            {
                DropServer(_heldItem);
                return;
            }

            PickupItem target = FindNearestItem();

            if (target != null)
            {
                PickupServer(target);
            }
        }

        private bool ReadInteractPressed()
        {
            bool pressed = false;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                pressed |= keyboard.eKey.wasPressedThisFrame;
            }

            Gamepad gamepad = Gamepad.current;

            if (gamepad != null)
            {
                pressed |= gamepad.buttonWest.wasPressedThisFrame;
            }

            return pressed;
        }

        private PickupItem FindNearestItem()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayer);

            PickupItem nearest = null;
            float nearestDistanceSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                PickupItem item = hit.GetComponent<PickupItem>();

                if (item == null || item.IsHeld)
                {
                    continue;
                }

                float distanceSqr = (hit.transform.position - transform.position).sqrMagnitude;

                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearest = item;
                }
            }

            return nearest;
        }

        [ServerRpc]
        private void PickupServer(PickupItem item)
        {
            if (item == null || item.IsHeld)
            {
                return;
            }

            item.PickUp(this, handSocket.localPosition, handSocket.localRotation);
            TargetSetHeldItem(Owner, item);
        }

        [ServerRpc]
        private void DropServer(PickupItem item)
        {
            if (item == null || !item.IsHeld)
            {
                return;
            }

            item.Drop();
            TargetSetHeldItem(Owner, null);
        }

        [TargetRpc]
        private void TargetSetHeldItem(NetworkConnection conn, PickupItem item)
        {
            _heldItem = item;
        }
    }
}
