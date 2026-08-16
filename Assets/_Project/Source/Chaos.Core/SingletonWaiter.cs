using System;
using System.Collections;

namespace Chaos.Core
{
    internal static class SingletonWaiter
    {
        public static IEnumerator WaitFor<T>(Func<T> getInstance, Action<T> onReady) where T : class
        {
            T instance = getInstance();

            while (instance == null)
            {
                yield return null;
                instance = getInstance();
            }

            onReady(instance);
        }
    }
}
