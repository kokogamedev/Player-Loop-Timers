using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using PsigenVision.Utilities.LowLevel;
using UnityEditor;
using UnityEngine.PlayerLoop;

namespace PsigenVision.PlayerLoopTimers
{
	internal static class TimerBootStrapper
	{
		//Wrap up the timer manager in a player loop system as its own subsystem
		private static PlayerLoopSystem timerSystem;

		/// <summary>
		/// Initializes the ImprovedTimers system by modifying the Unity Player Loop to insert the TimerManager
		/// as a subsystem of the Update loop. This ensures that the TimerManager's update logic is executed
		/// during the game loop.
		/// </summary>
		/// <remarks>
		/// This method is called automatically when assemblies are loaded during runtime and uses the
		/// PlayerLoop API to modify the Unity Player Loop system. The TimerManager is inserted at the first
		/// position of the Update subsystem. If the insertion fails, a warning is logged, and the system
		/// does not initialize.
		/// </remarks>
		/// <exception cref="UnityException">
		/// Thrown if the Unity Player Loop cannot be modified.
		/// </exception>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void Initialize()
		{
			//Cache the current state of the player loop
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			//Attempt insertion of the timer manager into the current cached player loop system as the first subsystem to be run under update
			if (!InsertTimerManager<UnityEngine.PlayerLoop.Update>(ref currentPlayerLoop, 0))
			{
				Debug.LogWarning("Could not insert TimerManager as a player loop subsystem of the Update loop; ImprovedTimers could not be initialized by the TimerBootStrapper");
				return;
			}
			
			//Set the modified (post-insertion) cached current player loop system to be the ACTUAL current player loop system that Unity is using
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
			
			//Debug - optional - Print player loop
			//PlayerLoopUtils.PrintPlayerLoop(currentPlayerLoop);
			
			//Ensure that when we come out of playmode, all clean up is performed (disposal and finalizing of the Time Manager class)
			//This is to prevent, for example, multiple TimeManager player loop systems being injected on entering playmode in the event automatic domain reload is not enabled
			//This is performed by hooking into the playmode state changed event
#if UNITY_EDITOR
			//Preemptively unsubscribe cleanup method from playModeStateChanged to prevent multiple subscriptions
			EditorApplication.playModeStateChanged -= OnPlayModeState;
			//Subscribe our cleanup method to the playModeStateChanged event
			EditorApplication.playModeStateChanged += OnPlayModeState;
#endif

			static void OnPlayModeState(PlayModeStateChange state)
			{
				//When exiting playmode, perform "cleanup" of our Improved Timer player loop system - disposal and finalizing
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					//Cache the current post-injection player loop 
					PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
					//Remove the injected TimerManager (Improved Timers) system from the cached player loop
					RemoveTimerManager<Update>(ref currentPlayerLoop);
					//Inject the cached "un-injected" (the player loop after removing the previously injected improved timers system) into the current player loop
					PlayerLoop.SetPlayerLoop(currentPlayerLoop);
					//Unity does not guarantee the clearing of statics, so we must manually ensure the static list of timers is cleared every time cleanup is performed
					//Similarly we must manually handle disposal and finalization (use disposal pattern)
					TimerManager.Clear();
				}
			}
		}
		
		//Methods required to define and insert the timer manager system into the player loop
		/// <summary>
		/// Inserts the TimerManager as a subsystem of a specified PlayerLoopSystem, enabling
		/// the TimerManager's update logic to be executed within the Unity game loop.
		/// </summary>
		/// <typeparam name="T">
		/// The PlayerLoopSystem type under which the TimerManager should be inserted (e.g., Update).
		/// </typeparam>
		/// <param name="loop">
		/// The current PlayerLoopSystem that defines the Unity game loop structure.
		/// </param>
		/// <param name="index">
		/// The position in the target subsystem's list where the TimerManager should be inserted.
		/// </param>
		/// <returns>
		/// Returns true if the TimerManager was successfully inserted into the specified PlayerLoopSystem;
		/// otherwise, returns false.
		/// </returns>
		static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
		{
			//This method is generic as T represents which system in the player loop we wish to serve as the root/parent of the subsystem we are inserting (e.g. Update)
			//index serves as an indicator for where in the subsystem list we wish to position the current subsystem
			timerSystem = new PlayerLoopSystem()
			{
				type = typeof(TimerManager), //type of the subsystem we are inserting
				updateDelegate =
					TimerManager.UpdateTimers, //what method in our class is going to be called to run the subystem
				subSystemList = null //what is the collection of subsystems possessed by this subsystem
			};
			
			//Insert the timer manager player loop system as a subsystem of the player loop system of type T within the current player loop
			if (!PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index))
			{
				Debug.LogWarning($"Unable to insert timer manager into Player Loop System at {typeof(T).Name}");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Removes the TimerManager system from the Unity Player Loop hierarchy by utilizing the PlayerLoopUtils.RemoveSystem method.
		/// This ensures that the cached TimerManager system is properly detached from the Player Loop graph.
		/// </summary>
		/// <typeparam name="T">
		/// The type parameter for specifying the PlayerLoopSystem, included for potential future use but not strictly required
		/// as the removal process traverses the entire Player Loop hierarchy.
		/// </typeparam>
		/// <param name="loop">
		/// A reference to the root PlayerLoopSystem from which the TimerManager system will be removed.
		/// </param>
		static void RemoveTimerManager<T>(ref PlayerLoopSystem loop)
		{
			//This is a wrapper around our helper method defined in PlayerLoopUtils for removing our cached timer system from the player loop graph
			//NOTE: technically the generic T is not needed as we are recursing the entire player loop graph - however, it is here for future-proofing
			PlayerLoopUtils.RemoveSystem<T>(ref loop, in timerSystem);
		}
	}
}