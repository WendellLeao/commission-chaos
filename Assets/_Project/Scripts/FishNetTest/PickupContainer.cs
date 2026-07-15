using FishNet.Object;
using UnityEngine;

namespace _Project
{
    internal sealed class PickupContainer : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServerInitialized)
            {
                return;
            }

            PlayerPickup player = other.GetComponentInParent<PlayerPickup>();

            if (player == null)
            {
                return;
            }

            PickupItem heldItem = player.GetComponentInChildren<PickupItem>();

            if (heldItem == null || !heldItem.IsHeld)
            {
                return;
            }

            player.ClearHeldItemServer();
            Despawn(heldItem.NetworkObject);
            ScoreManager.Instance.AddScore(ScoreManager.PointsPerItem);
        }
    }
}
