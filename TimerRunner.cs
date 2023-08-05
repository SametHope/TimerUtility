using System.Collections.Generic;
using UnityEngine;

namespace SametHope.TimerUtility
{
    /// <summary>
    /// A hidden MonoBehaviour singleton that initializes itself automatically. Responsible for updating alive <see cref="Timer"/> instances.
    /// </summary>
    public class TimerRunner : MonoBehaviour
    {
        /// <summary>
        /// Gets the instance of the <see cref="TimerRunner"/> singleton.
        /// </summary>
        internal static TimerRunner Instance { get; private set; }

        /// <summary>
        /// Initializes the <see cref="TimerRunner"/> singleton instance and ensures it persists across scene changes.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Instance = new GameObject("[TimerRunner]").AddComponent<TimerRunner>();
            DontDestroyOnLoad(Instance);
            Instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// When a Timer is in this collection, it is assumed Alive and updated each frame according to its update method.
        /// <para>Do not add to or remove from this collection. Timers are added and removed automatically when they start and get killed.</para>
        /// </summary>
        public static List<Timer> AliveTimers => Instance._aliveTimers;
        public readonly List<Timer> _aliveTimers = new();

        private void Update()
        {
            for (int i = 0; i < _aliveTimers.Count; i++)
            {
                if (_aliveTimers[i].UpdateMethod == TimerUpdateMethod.NormalUpdate) UpdateTimer(_aliveTimers[i]);
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < _aliveTimers.Count; i++)
            {
                if (_aliveTimers[i].UpdateMethod == TimerUpdateMethod.FixedUpdate) UpdateTimer(_aliveTimers[i]);
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < _aliveTimers.Count; i++)
            {
                if (_aliveTimers[i].UpdateMethod == TimerUpdateMethod.LateUpdate) UpdateTimer(_aliveTimers[i]);
            }
        }

        private void UpdateTimer(Timer timer)
        {
            timer.Update(Time.deltaTime);
        }
    }
}