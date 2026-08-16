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
        [SerializeField] private float extraGravityMultiplier = 2.5f;

        private float _hopCooldown;
        private float _heldTimer;
        private bool _isGrounded;

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

        private void FixedUpdate()
        {
            _isGrounded = false;

            if (!IsServerInitialized || pickupItem.IsHeld)
            {
                return;
            }

            rb.AddForce(Physics.gravity * extraGravityMultiplier, ForceMode.Acceleration);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    _isGrounded = true;
                    return;
                }
            }
        }

        private void TickHeldTimer()
        {
            _heldTimer += Time.deltaTime;

            if (_heldTimer < autoDropDelay)
            {
                return;
            }

            _heldTimer = 0f;
            CharacterPickup holder = pickupItem.Holder;
            pickupItem.Drop();
            holder.ClearHeldItemServer();
        }

        private void TickFleeing()
        {
            Transform nearestCharacter = FindNearestCharacterWithinRadius();

            if (nearestCharacter == null)
            {
                return;
            }

            _hopCooldown -= Time.deltaTime;

            if (_hopCooldown > 0f || !_isGrounded)
            {
                return;
            }

            _hopCooldown = hopInterval;
            Hop(nearestCharacter.position);
        }

        private void Hop(Vector3 characterPosition)
        {
            Vector3 awayDirection = transform.position - characterPosition;
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

        private Transform FindNearestCharacterWithinRadius()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

            Transform nearest = null;
            float nearestDistanceSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                CharacterPickup character = hit.GetComponentInParent<CharacterPickup>();

                if (character == null)
                {
                    continue;
                }

                float distanceSqr = (character.transform.position - transform.position).sqrMagnitude;

                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearest = character.transform;
                }
            }

            return nearest;
        }
    }
}
