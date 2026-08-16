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

            CharacterPickup character = other.GetComponentInParent<CharacterPickup>();

            if (character == null)
            {
                return;
            }

            PickupItem heldItem = character.GetComponentInChildren<PickupItem>();

            if (heldItem == null || !heldItem.IsHeld)
            {
                return;
            }

            character.ClearHeldItemServer();
            Despawn(heldItem.NetworkObject);
            ScoreManager.Instance.AddScore(ScoreManager.PointsPerItem);
        }
    }
}
