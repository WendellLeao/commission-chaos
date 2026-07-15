using FishNet.Object;
using UnityEngine;

namespace _Project
{
    internal sealed class ChaoticFleeingItem : NetworkBehaviour
    {
        [SerializeField] private PickupItem pickupItem;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float detectionRadius = 4f;
        [SerializeField] private float hopForce = 3f;
        [SerializeField] private float hopInterval = 0.45f;
        [SerializeField] private float zigzagAngle = 40f;
        [SerializeField] private float autoDropDelay = 6f;

        private float _hopCooldown;
        private float _heldTimer;

        private void Update()
        {
            if (!IsServerInitialized)
            {
                return;
            }

            if (pickupItem.IsHeld)
            {
                TickHeldTimer();
                return;
            }

            _heldTimer = 0f;
            TickFleeing();
        }

        private void TickHeldTimer()
        {
            _heldTimer += Time.deltaTime;

            if (_heldTimer < autoDropDelay)
            {
                return;
            }

            _heldTimer = 0f;
            PlayerPickup holder = pickupItem.Holder;
            pickupItem.Drop();
            holder.ClearHeldItemServer();
        }

        private void TickFleeing()
        {
            Transform nearestPlayer = FindNearestPlayerWithinRadius();

            if (nearestPlayer == null)
            {
                return;
            }

            _hopCooldown -= Time.deltaTime;

            if (_hopCooldown > 0f)
            {
                return;
            }

            _hopCooldown = hopInterval;
            Hop(nearestPlayer.position);
        }

        private void Hop(Vector3 playerPosition)
        {
            Vector3 awayDirection = transform.position - playerPosition;
            awayDirection.y = 0f;

            if (awayDirection.sqrMagnitude < 0.0001f)
            {
                awayDirection = Random.insideUnitSphere;
                awayDirection.y = 0f;
            }

            awayDirection.Normalize();

            float randomAngle = Random.Range(-zigzagAngle, zigzagAngle);
            Vector3 hopDirection = Quaternion.Euler(0f, randomAngle, 0f) * awayDirection;
            Vector3 velocity = hopDirection * hopForce + Vector3.up * hopForce;

            rb.linearVelocity = velocity;
        }

        private Transform FindNearestPlayerWithinRadius()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

            Transform nearest = null;
            float nearestDistanceSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                PlayerPickup player = hit.GetComponentInParent<PlayerPickup>();

                if (player == null)
                {
                    continue;
                }

                float distanceSqr = (player.transform.position - transform.position).sqrMagnitude;

                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearest = player.transform;
                }
            }

            return nearest;
        }
    }
}
