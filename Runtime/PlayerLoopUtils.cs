using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;
using UnityEngine;

namespace PsigenVision.Utilities.LowLevel
{
	/// <summary>
	/// Provides utility methods for working with the Unity Player Loop system.
	/// </summary>
	public class PlayerLoopUtils
	{

		#region Player Loop System Insertion

		//Methods for insert a system into the player loop
		/// <summary>
		/// Inserts a PlayerLoopSystem into the specified Player Loop at the given index, under the subsystem of type T.
		/// </summary>
		/// <typeparam name="T">The type of the PlayerLoopSystem under which the system will be inserted.</typeparam>
		/// <param name="loop">The PlayerLoopSystem to modify by inserting the specified system.</param>
		/// <param name="systemToInsert">The PlayerLoopSystem to insert into the Player Loop.</param>
		/// <param name="index">The index where the system will be inserted in the subsystem list of the specified type.</param>
		/// <returns>True if the system was successfully inserted; otherwise, false.</returns>
		public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
		{
			//PrintPlayerLoop(loop);
			//Debug.Log($"System to insert is: {systemToInsert.type} as a subsystem of {typeof(T)} where the inserted loop is of type {loop.type}");
			//use recursion to find where to insert the system
			//if we have passed in the root of our subsystem, we check if that loop is of type T until we find a match (the system in the player loop we wish to serve as the root/parent)
			if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);
			
			//Match for our desired root system of type T found during graph-search type-checking recursive phase
			//Inject systemToInsert into the player loop graph
			
			//Cache all subsystems of our root system of type T
			var playerLoopSubsystemList = new List<PlayerLoopSystem>();
			//If the subsystem list is not null (there are existing subsystems for the found root system), cache that collection
			if (loop.subSystemList != null) playerLoopSubsystemList.AddRange(loop.subSystemList);
			//Insert the subsystem we defined (systemToInsert) at the index we selected (index) within the cached subsystem list - results in a subsystem list in the correct sequence that can be injected 
			playerLoopSubsystemList.Insert(index, systemToInsert);
			//Inject this newly constructed subsystem list into the subsystem of our selected root player loop system of type T
			loop.subSystemList = playerLoopSubsystemList.ToArray();
			return true;
		}

		/// <summary>
		/// Handles the recursive traversal of the Player Loop System graph to locate a root subsystem of the specified type and attempts to insert the given system into it.
		/// </summary>
		/// <param name="loop">The current PlayerLoopSystem node being traversed in the graph.</param>
		/// <param name="systemToInsert">The PlayerLoopSystem instance to insert as a subsystem if the appropriate root is found.</param>
		/// <param name="index">The index at which the system should be inserted in the subsystem list of the root system.</param>
		/// <typeparam name="T">The type of the root subsystem to locate within the Player Loop System hierarchy.</typeparam>
		/// <returns>True if the system was successfully inserted in a subsystem of the specified type; otherwise, false.</returns>
		private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert,
			int index)
		{
			PrintPlayerLoopSystem(loop);
			//check if we have reached a leaf in the player loop system graph (no more subsystems to recurse)
			if (loop.subSystemList == null || loop.subSystemList.Length == 0) return false;
			//go over each subsystem that belongs to this particular node in the player loop graph, and check if its type matches T (the root system we are looking for)
			for (int i = 0; i < loop.subSystemList.Length; i++)
			{
				//Debug.Log($"System to insert is: {systemToInsert.type} as a subsystem of {typeof(T)} where the inserted subsystem loop is of type {loop.subSystemList[i].type}");
				//Try again to insert this system as a subsystem of root system of type T
				//If this fails, continue to the next subsystem
				//If this succeeds, InsertSystem proceeds past this recursive root system search phase and enters the actual insertion phase - an insertion of the systemToInsert system as a subsystem of the root system that matches type T
				if (InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) return true;
			}
			
			//if InsertSystem consistently returned false, there is no system of type T in the player loop graph, and we return false to reflect that
			Debug.LogWarning($"No SubSystem Loop of type {typeof(T)} found in the player loop graph");
			return false;
		}

		#endregion
		
		//Below method is a standard way of inserting a manager as a system; however, C# prevents usage of static types as generic type parameters
		/*public static bool InsertManagerAsSystem<RootLoopType, ManagerStaticType>(ref PlayerLoopSystem currentLoop, int index,
			PlayerLoopSystem.UpdateFunction updateDelegate, PlayerLoopSystem[] subSystemList, out PlayerLoopSystem managerAsSystem)
		{
			//This method is generic as RootLoopType represents which system in the player loop we wish to serve as the root/parent of the subsystem we are inserting (e.g. Update)
			//index serves as an indicator for where in the subsystem list we wish to position the current subsystem
			managerAsSystem = new PlayerLoopSystem()
			{
				type = typeof(ManagerStaticType), //type of the subsystem we are inserting
				updateDelegate =
					updateDelegate, //what method in our class is going to be called to run the subystem
				subSystemList = subSystemList //what is the collection of subsystems possessed by this subsystem
			};

			//Insert the timer manager player loop system as a subsystem of the player loop system of type T within the current player loop
			return PlayerLoopUtils.InsertSystem<RootLoopType>(ref currentLoop, in managerAsSystem, index);
		}*/

		#region Player Loop System Removal/Cleanup

		//Remove systems from the player loop
		/// <summary>
		/// Removes the specified system from the Player Loop system hierarchy recursively starting at the given root loop.
		/// </summary>
		/// <param name="loop">The root PlayerLoopSystem to traverse and attempt removal of the specified system.</param>
		/// <param name="systemToRemove">The PlayerLoopSystem instance to remove from the Player Loop hierarchy.</param>
		/// <typeparam name="T">The type of the PlayerLoopSystem to consider for removal.</typeparam>
		public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
		{
			//NOTE: there is no reason to provide a specific index for system removal, as we essentially only need to walk the player loop graph until we find the systemToRemove
			//Debug.Log($"System to remove is: {systemToRemove.type} where the inserted loop is of type {loop.type}");
			//First, check if there are no subsystems of the loop from we wish to remove the specified subsystem (i.e. if we are at a leaf), and bail out if that is the case
			if (loop.subSystemList == null || loop.subSystemList.Length == 0) return;
			
			//Cache a copy of the current player loop for potential modification (system removal) and injection (applying the system removal to unity's actual player loop)
			var playerLoopSubsystemList = new List<PlayerLoopSystem>(loop.subSystemList);
			
			//Walk through the subgraph of player loop systems beneath the specified root player loop from which we want to remove the specified systemToRemove, and search for that systemToRemove (by type)
			for (int i = 0; i < playerLoopSubsystemList.Count; i++)
			{
				var currentSystem = playerLoopSubsystemList[i];
				//If the current system's type or update delegate do not match that of the systemToRemove, continue onto the next subsystem
				if (currentSystem.type != systemToRemove.type || currentSystem.updateDelegate != systemToRemove.updateDelegate) continue;
				//At this point, we have found the systemToRemove, so we remove it at the specified index
				playerLoopSubsystemList.RemoveAt(i);
				//Inject the modified player loop subsystem list (post-system-removal) into the current root loop as its subsystem list
				loop.subSystemList = playerLoopSubsystemList.ToArray();
			}
			
			//Potentially, this systemToRemove could have been inserted as a subsystem of any one of the root system's own subsystems; therefore we must check for the presence of the systemToRemove and if found remove it in these cases as well.
			HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
		}

		/// <summary>
		/// Handles the recursive removal of a specific PlayerLoopSystem from the subsystem list of a specified PlayerLoopSystem.
		/// </summary>
		/// <param name="loop">The PlayerLoopSystem whose subsystems will be checked and potentially modified.</param>
		/// <param name="systemToRemove">The PlayerLoopSystem to be removed if found within the subsystem list.</param>
		/// <typeparam name="T">The type parameter used for identifying the system hierarchy during recursive traversal.</typeparam>
		private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop,
			in PlayerLoopSystem systemToRemove)
		{
			//If the subsystem list is null or empty, we can just bail out since we are at a leaf in the player loop graph
			if (loop.subSystemList == null || loop.subSystemList.Length == 0) return;
			
			//Walk through the subsystem list of player loop systems beneath the specified root player loop subsystem from which we want to remove the specified systemToRemove, and search for that systemToRemove (by type)
			for (int i = 0; i < loop.subSystemList.Length; i++)
			{
				//Try again to remove this system as a subsystem from root system of type T
				//If this fails, continue to the next subsystem
				//If this succeeds, RemoveSystem proceeds past this recursive root system search phase and enters the actual removal phase - a removal of the systemToRemove system as a subsystem from the root system that matches type T
				RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
			}
		}

		#endregion

		#region Printing (Debugging)

		/// <summary>
		/// Prints the hierarchy of the Unity Player Loop system starting from the specified root loop.
		/// </summary>
		/// <param name="rootLoop">The root PlayerLoopSystem from which to begin traversal and print the Player Loop hierarchy.</param>
		public static void PrintPlayerLoop(PlayerLoopSystem rootLoop)
		{
			//Recursively traverses the player loop graph and outputs the result to the console
			
			StringBuilder sb = new();
			sb.AppendLine("Unity Player Loop");
			//Iterate over ever PlayerLoopSystem subsystem in this root system's subSystemList array
			foreach (PlayerLoopSystem subSystem in rootLoop.subSystemList)
				PrintSubsystem(subSystem, sb, 0);
			
			//Print the player loop graph to the console
			Debug.Log(sb.ToString());

			//Iterate recursively through the subsystems and print them into the stringbuilder variable
			//NOTE: level is the level we are at in our recursion
			static void PrintSubsystem(PlayerLoopSystem parentSystem, StringBuilder sb, int level)
			{
				if (level == 0)
				{
					sb.AppendLine("SYSTEM TYPE: root (null)"); //print the current system type
				}
				else
				{
					sb.Append(' ', level * 2) //indent relative to the current recursion level 
						.AppendLine(parentSystem.type.ToString()); //print the current system type
				}
				
				//if this is a leaf in the graph, and we have nowhere else to go, return
				if (parentSystem.subSystemList == null || parentSystem.subSystemList.Length == 0) return;
				
				//otherwise, iterate over every subsystem in this particular node, and recursively traverse the graph
				foreach (var subSystem in parentSystem.subSystemList)
				{
					PrintSubsystem(subSystem, sb, level + 1);
				}
			}
		}

		public static void PrintPlayerLoopSystem(PlayerLoopSystem loop)
		{
			StringBuilder sb = new();
			if (loop.type == null)
			{
				sb.AppendLine("SYSTEM TYPE: null"); //print the current system type
			}
			else
			{
				sb.Append("SYSTEM TYPE: ")
					.AppendLine(loop.type.ToString()); //print the current system type
			}
				
			//if this is a leaf in the graph, and we have nowhere else to go, return
			if (loop.subSystemList == null || loop.subSystemList.Length == 0) return;
				
			sb.AppendLine("SUBSYSTEMS:");
			//otherwise, iterate over every subsystem in this particular node, and recursively traverse the graph
			foreach (var subSystem in loop.subSystemList)
			{
				sb.Append(' ').AppendLine(subSystem.type.ToString()); //print the current system type
			}
		}

		#endregion
	}
}
