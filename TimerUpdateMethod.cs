namespace SametHope.TimerUtility
{
    /// <summary>
    /// Enumeration for various update methods used by <see cref="Timer"/>.
    /// </summary>
    [System.Serializable]
    public enum TimerUpdateMethod
    {
        /// <summary>
        /// Update on every <c>Update()</c> call of unity.
        /// </summary>
        NormalUpdate,

        /// <summary>
        /// Update on every <c>LateUpdate()</c> call of unity.
        /// </summary> 
        LateUpdate,

        /// <summary>
        /// Update on every <c>FixedUpdate()</c> call of unity.
        /// </summary>
        FixedUpdate,

        /// <summary>
        /// Update with <see cref="Timer.ManualUpdate(float)"/> calls.
        /// </summary>
        ManualUpdate
    }
}