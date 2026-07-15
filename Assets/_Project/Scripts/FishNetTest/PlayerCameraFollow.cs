using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project
{
    public class PlayerCameraFollow : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;

            if (!IsOwner)
            {
                return;
            }

            CinemachineCamera playerCamera = FindFirstObjectByType<CinemachineCamera>();
            playerCamera.Follow = transform;
        }
    }
}
