using UnityEngine;

namespace PsigenVision.PlayerLoopTimers
{
    /// <summary>
    /// Represents a countdown timer that decrements over time.
    /// The timer supports starting, pausing, resuming, stopping, and resetting functionality.
    /// </summary>
    /// <remarks>
    /// This class provides mechanisms to track elapsed time and determine when the countdown has completed.
    /// It operates by decrementing the current time each frame until it reaches zero or below, at which point the timer stops.
    /// </remarks>
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float initialTime) : base(initialTime) { }

        /// <summary>
        /// Updates the state of the timer during each frame. If the timer is not running, the method exits without performing any action.
        /// If the timer is running and the current time is positive, it decreases the current time by the elapsed time.
        /// If the timer is running but the current time has reached zero or a negative value, the timer is stopped.
        /// </summary>
        public override void Update()
        {
            //If the timer is not running, bail out (do not perform an update/tick)
            if (!IsRunning) return;
            //If the timer is running and not finished (CurrentTime is positive) decrement CurrentTime
            if (CurrentTime > 0) CurrentTime -= Time.deltaTime;
            //If the timer is running and IS finished (Current time is negative) Stop timer with the finished state (thereby firing the OnTimerFinish callback)
            else Stop(true);
        }

        /// <summary>
        /// Determines whether the timer has completed its intended operation (countdown is complete).
        /// </summary>
        public override bool IsFinished => CurrentTime > 0f;
    }
}