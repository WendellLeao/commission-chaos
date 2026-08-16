using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace _Project
{
    internal sealed class PickupSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject pickupPrefab;
        [SerializeField] private NetworkObject chaoticFleeingPrefab;
        [SerializeField] private BoxCollider spawnBounds;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxConcurrentItems = 5;
        [SerializeField] [Range(0f, 1f)] private float chaoticSpawnChance = 0.25f;

        private readonly List<NetworkObject> _activeItems = new();

        public override void OnStartServer()
        {
            base.OnStartServer();

            InvokeRepeating(nameof(TrySpawnItem), spawnInterval, spawnInterval);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            CancelInvoke(nameof(TrySpawnItem));
        }

        private void TrySpawnItem()
        {
            _activeItems.RemoveAll(item => item == null);

            if (_activeItems.Count >= maxConcurrentItems)
            {
                return;
            }

            NetworkObject prefab = RollPrefab();
            Vector3 spawnPosition = GetRandomSpawnPosition();
            NetworkObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            Spawn(instance);
            _activeItems.Add(instance);
        }

        private NetworkObject RollPrefab()
        {
            bool spawnChaotic = Random.value < chaoticSpawnChance;

            return spawnChaotic ? chaoticFleeingPrefab : pickupPrefab;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Bounds bounds = spawnBounds.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(x, bounds.center.y, z);
        }
    }
}
