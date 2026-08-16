using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project
{
    public class CharacterCameraFollow : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;

            if (!IsOwner)
            {
                return;
            }

            CinemachineCamera characterCamera = FindFirstObjectByType<CinemachineCamera>();
            characterCamera.Follow = transform;
        }
    }
}
