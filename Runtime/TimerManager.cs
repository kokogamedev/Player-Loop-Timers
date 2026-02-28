using System.Collections.Generic;

namespace PsigenVision.ImprovedTimers
{
    public static class TimerManager
    {
        private static readonly List<Timer> timers = new(); //manager requires a list of all active timers in the game
        
        //manager requires a way for timers to add or remove themselves
        public static void RegisterTimer(Timer timer) => timers.Add(timer);

        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static void UpdateTimers()
        {
            //Iterate through all registered timers and make them tick (this is hooked in as a subsystem of the update player loop system)
            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].Update();
            }
        }
        
        public static void Clear() => timers.Clear();
    }
}