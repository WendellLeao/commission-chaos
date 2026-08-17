using UnityEngine;

namespace Chaos.Core
{
    public sealed class PersistentObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
