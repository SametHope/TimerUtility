using System;
using UnityEngine;

namespace SametHope.TimerUtility
{
    /// <summary>
    /// Represents a timer utility that can be used to schedule and manage timed events.
    /// </summary>
    [Serializable]
    public class Timer
    {
        /// <summary>
        /// Default value for <see cref="CompleteTime"/>.
        /// <para>Also used in <see cref="GetNormalizedTime()"/> method to check if the <see cref="CompleteTime"/> of the instance has changed.</para>
        /// </summary>
        public const float DEFAULT_COMPLETE_TIME = Mathf.Infinity;

        /// <summary>
        /// Default value for <see cref="UpdateMethod"/>.
        /// </summary>
        public const TimerUpdateMethod DEFAULT_UPDATE_METHOD = TimerUpdateMethod.NormalUpdate;

        /// <summary>
        /// A string to differentiate between different instances of timers. 
        /// <para>This is not used internally but is here for external usage.
        /// <br></br>For example you could access (and stop if you want to) a timer called "MyTimer" like this: <code>TimerRunner.AliveTimers.Find(t => t.Name == "MyTimer")?.Stop();</code></para>
        /// </summary>
        [field: SerializeField] public string Name { get; set; }

        /// <summary>
        /// The method to update the timer instance.
        /// <para>It is not recommended to change this after starting the timer.</para>
        /// </summary>
        [field: SerializeField] public TimerUpdateMethod UpdateMethod { get; set; } = DEFAULT_UPDATE_METHOD;

        /// <summary>
        /// How many seconds have passed while this timer has been running.
        /// </summary>
        [field: SerializeField, TimerEditorReadOnly] public float Elapsed { get; private set; }

        /// <summary>
        /// How many seconds should the timer take to complete.
        /// <para>This is infinity by default, meaning the timer will run until it has been manually completed, stopped or killed.</para>
        /// </summary>
        [field: SerializeField] public float CompleteTime { get; set; } = DEFAULT_COMPLETE_TIME;

        /// <summary>
        /// When this is true, the timer will stop updating until it is set false again.
        /// <para>Use <see cref="Pause"/> and <see cref="Unpause"/> to set.</para>
        /// </summary>
        [field: SerializeField, TimerEditorReadOnly] public bool IsPaused { get; private set; }

        /// <summary>
        /// Has this instance started and yet to be killed?
        /// <para>Use <see cref="Pause"/> and <see cref="Unpause"/> to set.</para>
        /// <para>Note: Use Timer.
        /// </summary>
        [field: SerializeField, TimerEditorReadOnly] public bool IsAlive { get; private set; }

        #region Events
        /// <summary>
        /// Gets invoked when <see cref="Start"/> is called.
        /// </summary>
        public event Action OnStart;

        /// <summary>
        /// Gets invoked each frame the instance updates.
        /// </summary>
        public event Action OnUpdate; // only when update

        /// <summary>
        /// Gets invoked when the instance is complete.
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Gets invoked only when <see cref="Stop"/> is called.
        /// </summary>
        public event Action OnStop;

        /// <summary>
        /// Gets invoked when the instance is stopped, completed or manually killed.
        /// </summary>
        public event Action OnKill;

        /// <summary>
        /// Gets invoked when <see cref="Pause"/> is called.
        /// </summary>
        public event Action OnPause;

        /// <summary>
        /// Gets invoked when <see cref="Unpause"/> is called.
        /// </summary>
        public event Action OnUnpause;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// <para>There are no constructor with parameters as it is recommended to set the properties you need after initializing the instance.</para>
        /// </summary>
        public Timer() { }

        /// <summary>
        /// Gets the normalized time value for easing/tweening. Ideally using <see cref="TimerUtilities.EasingFunctions"/> which is a collection of easing functions.
        /// </summary>
        /// <remarks>
        /// The normalized time value represents how much of the completion time has passed in the range of [0, 1].
        /// It is useful for tweening or other time-based calculations.
        /// </remarks>
        /// <returns>The normalized time value between 0 and 1. Returns 0 if the CompleteTime is not set or set to infinity.</returns>
        public float GetNormalizedTime()
        {
            if (CompleteTime == DEFAULT_COMPLETE_TIME)
            {
                Debug.LogWarning("You should not use GetNormalizedTime() method or intend to tween if the CompleteTime is not set or is set to infinite.");
                return 0f;
            }

            return Mathf.Clamp01(Elapsed / CompleteTime);
        }

        /// <summary>
        /// Start the instance. 
        /// </summary>
        public void Start()
        {
            if (IsAlive)
            {
                Debug.LogWarning("You can not start a timer when it has already been started.");
                return;
            }

            TimerRunner.Instance._aliveTimers.Add(this);
            IsAlive = true;

            OnStart?.Invoke();
        }

        /// <summary>
        /// Will pause the instance.
        /// </summary>
        public void Pause()
        {
            if (IsPaused)
            {
                Debug.LogWarning("You can not pause a timer when it is already paused.");
                return;
            }

            IsPaused = true;
            OnPause?.Invoke();
        }

        /// <summary>
        /// Will unpause the instance.
        /// </summary>
        public void Unpause()
        {
            if (!IsPaused)
            {
                Debug.LogWarning("You can not unpause a timer when it is already running.");
                return;
            }

            IsPaused = false;
            OnUnpause?.Invoke();
        }

        /// <summary>
        /// Complete and kill the instance manually.
        /// </summary>
        /// <param name="updateElapsedTime">Should <see cref="Elapsed"/> be set to <see cref="CompleteTime"/>.</param>
        public void Complete(bool updateElapsedTime = false)
        {
            if (updateElapsedTime)
            {
                Elapsed = CompleteTime;
            }

            OnComplete?.Invoke();
            Kill();
        }

        /// <summary>
        /// Stop and kill the instance without completing it.
        /// </summary>
        public void Stop()
        {
            OnStop?.Invoke();
            Kill();
        }

        /// <summary>
        /// Kill the instance manually. 
        /// </summary>
        public void Kill()
        {
            if (!IsAlive)
            {
                Debug.LogWarning("You can not kill a timer that is not alive.");
                return;
            }

            _ = TimerRunner.Instance._aliveTimers.Remove(this);
            IsAlive = false;

            OnKill?.Invoke();
        }

        /// <summary>
        /// Manually update the instance.
        /// <para>Valid if <see cref="UpdateMethod"/> is set to <see cref="UpdateType.Manual"/>.</para>
        /// <para>
        /// <code>
        /// // Example usage for manual calls; You could start this Coroutine to update on each interval you want.
        /// private IEnumerator Co_ManualUpdateRepeater(Timer timer, float updateInterval)
        /// {
        ///     WaitForSeconds yielder = new WaitForSeconds(updateInterval);
        ///     while (timer.IsAlive)
        ///     {
        ///         yield return yielder;
        ///         if(timer.IsAlive) timer.ManualUpdate(updateInterval);
        ///     }
        /// }
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="deltaTime">Amount of time to increase <see cref="Elapsed"/> with.</param>
        public void ManualUpdate(float deltaTime)
        {
            if (UpdateMethod != TimerUpdateMethod.ManualUpdate)
            {
                Debug.LogWarning($"You can not call ManualUpdate() while UpdateType is set to '{UpdateMethod}'. It must be set to '{TimerUpdateMethod.ManualUpdate}'.");
                return;
            }

            Update(deltaTime);
        }

        internal void Update(float deltaTime)
        {
            if (!IsAlive)
            {
                Debug.LogWarning("Trying to update a timer instance that is not alive!");
                return;
            }
            if (IsPaused)
            {
                return;
            }

            Elapsed += deltaTime;

            if (Elapsed > CompleteTime)
            {
                Elapsed = CompleteTime;
            }

            OnUpdate?.Invoke();

            if (Elapsed == CompleteTime)
            {
                // No need to set true but lets be explicit anyway
                Complete(true);
            }
        }
    }
}
