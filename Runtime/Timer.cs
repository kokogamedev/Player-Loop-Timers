using System;
using UnityEngine;

namespace PsigenVision.ImprovedTimers
{
    /// <summary>
    /// Represents an abstract base class for a customizable timer.
    /// The Timer class provides functionality for tracking time, managing timer states,
    /// and updating the timer based on specific logic defined in its derived classes.
    /// </summary>
    /// <remarks>
    /// This class includes mechanisms for pausing, starting, stopping, and resetting the timer.
    /// Additionally, it provides timing progress, an initialization mechanism, and lifecycle hooks
    /// such as start and stop callbacks.
    /// </remarks>
    /// <example cref="Dispose"/>
    public abstract class Timer : IDisposable
    {
        /// <summary>
        /// Gets the current value of the timer in seconds.
        /// Represents the elapsed time or value being tracked by the timer.
        /// </summary>
        public float CurrentTime { get; protected set; }

        /// <summary>
        /// Indicates whether the timer is actively running.
        /// Represents the operational state of the timer, where true signifies
        /// it is in progress, and false indicates it is paused or stopped.
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Represents the initial time value of the timer in seconds.
        /// This is the predefined starting point from which the timer begins tracking time.
        /// </summary>
        protected float initialTime;

        /// <summary>
        /// Gets the normalized progress of the timer as a value between 0 and 1.
        /// Calculated as the current timer value divided by the initial time and clamped within the valid range.
        /// </summary>
        public float Progress =>
            Mathf.Clamp01(CurrentTime / initialTime);

        /// <summary>
        /// Represents an event that is triggered when the timer starts.
        /// This callback action is invoked after successfully initiating the timer's operation.
        /// Typically used for executing logic tied to the start of the timer, such as UI updates or other side effects.
        /// </summary>
        public Action OnTimerStart = delegate { };

        /// <summary>
        /// Represents an event that is triggered when the timer stops.
        /// This callback action is invoked after the timer's operation is successfully halted.
        /// Typically used to execute logic associated with the timer stopping, such as finalizing
        /// operations, updating UI, or other related functionality.
        /// </summary>
        public Action OnTimerStop = delegate { };

        public Timer(float initialTime) => this.initialTime = initialTime;
        
        /// <summary>
        /// Starts the timer by setting its current time to the initial value and registering it with the TimerManager.
        /// If the timer is not already running, it changes its state to running and triggers the OnTimerStart callback.
        /// </summary>
        public void Start()
        {
            //Set the current time to the cached initial time
            CurrentTime = initialTime;
            //If the timer is currently running, set the IsRunning state to false, deregister this timer with the timer manager, and fire off the OnTimerStop event
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        /// <summary>
        /// Stops the timer by changing its state to not running and removing it from the TimerManager.
        /// If the timer is currently running, it deregisters itself from the TimerManager and
        /// triggers the OnTimerStop callback to notify listeners that the timer has stopped.
        /// </summary>
        public void Stop()
        {
            //If the timer is not already running, set the IsRunning state to true, register this timer with the timer manager, and fire off the OnTimerStart event
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }

        /// <summary>
        /// Resumes the timer by setting its running state to true.
        /// This enables the timer to continue tracking time from its current state.
        /// Typically used after the timer has been paused to allow it to proceed without resetting.
        /// </summary>
        public void Resume() => IsRunning = true;

        /// <summary>
        /// Pauses the timer by setting its running state to false.
        /// This halts the timer's progression without resetting its current time,
        /// allowing a subsequent resume to continue from the paused state.
        /// </summary>
        public void Pause() => IsRunning = false;

        #region Overridable/Abstract Members

        /// <summary>
        /// Resets the timer by setting the current time to the initial time value.
        /// This method reinitializes the timer state without altering whether it is running or stopped.
        /// Useful for restarting the timer from its predefined initial value.
        /// </summary>
        public virtual void Reset() => CurrentTime = initialTime;

        /// <summary>
        /// Resets the timer by assigning a new initial time value and setting the current time to match the new initial value.
        /// This method is useful for reinitializing the timer with a different starting time while maintaining its functionality.
        /// </summary>
        /// <param name="newTime">The new initial time value to set for the timer.</param>
        public virtual void Reset(float newTime)
        {
            //useful for reusing an old timer with a new initial time
            initialTime = newTime;
            Reset();
        }

        /// <summary>
        /// Executes logic to update the state of the timer. This method is meant to be implemented
        /// by derived classes to define specific behaviors that occur each time a timer update is triggered.
        /// Typically called by a timer management system to ensure consistent timer progression across
        /// the application.
        /// </summary>
        public abstract void Update(); //tick timer every update

        /// <summary>
        /// Gets a value indicating whether the timer has completed its operation.
        /// Represents the state of the timer, where true indicates the timer has finished
        /// and false indicates it is still running or in progress.
        /// </summary>
        public abstract bool
            IsFinished
        {
            get;
        } //each timer implementer is responsible for providing their own definition for the finished state of the timer (e.g. countdown timer and stopwatch timer have different finishing conditions)

        #endregion

        #region Disposal

        bool _disposed = false; //disposal state flag designed to prevent repeated disposal

        ~Timer() //finalizer (for GC)
        {
            /*
             * A finalizer is a safety net (called by the GC if `Dispose()` is not explicitly called)
             * that ensures unmanaged resources are released. It calls `Dispose(false)`.
             * It should not be used if the class only holds managed resources.
             */
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the Timer instance. This includes both managed and unmanaged resources.
        /// When this method is called, it ensures that the Timer object is properly deregistered from the TimerManager
        /// and that its internal state is cleaned up. It also suppresses the finalization of the object to optimize garbage collection.
        /// </summary>
        public void Dispose()
        {
            /*
             * This public, non-virtual method is called by the consumer to explicitly release resources as soon as they are
             * done with the object. It calls the protected virtual `Dispose(bool disposing)` method with `true` and then
             * calls `GC.SuppressFinalize(this)` to tell the garbage collector (GC) that the finalizer (destructor) is not needed.
             */
            //Call Dispose to ensure deregistration of the timer from the TimeManager
            //when consumer is done with the timer or being destroyed
            Dispose(true);
            GC.SuppressFinalize(
                this); //tells garbage collector that it does not need to call the finalizer since all resources (including unmanaged resources) have been freed
        }

        /// <summary>
        /// Releases resources used by the Timer object. It ensures proper cleanup of both managed and unmanaged resources.
        /// Prevents multiple calls to free resources and suppresses the finalizer when called explicitly.
        /// </summary>
        /// <param name="disposing">
        /// A boolean indicating the context of the call:
        /// - When true, indicates explicit disposal and ensures cleanup of both managed and unmanaged resources.
        /// - When false, indicates invocation by the finalizer and only unmanaged resources are cleaned.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            /*
             * This protected, virtual method contains the core cleanup logic. The `disposing` parameter determines the origin of the call:
                - If `true` (called from `Dispose()`), both managed resources (other disposable objects) and unmanaged resources should be cleaned up.
                - If `false` (called from the finalizer), only unmanaged resources should be cleaned up, as the GC handles managed resources in this scenario.
             */

            //finalizer utilizes this method where disposing is false (this is because finalizer only frees unmanaged resources)
            //disposer utilizes this method where disposing is true (this is because disposal involves freeing managed and unmanaged resources
            if (_disposed)
                return; //prevent repeated disposals (i.e. ensure disposal only happens once)

            if (disposing)
            {
                //Dispose managed state (managed objects)
                TimerManager
                    .DeregisterTimer(
                        this); //ensure Timer Manager does not retain a pointer to this timer by deregistering it
            }

            //Free unmanaged resources (unmanaged objects)

            _disposed = true;
        }

        #endregion
    }
}