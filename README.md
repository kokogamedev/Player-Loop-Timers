# Player Loop Timers

**An extensible Timer solution for Unity Game Development.**

v By leveraging Unity's Player Loop, timers are seamlessly integrated into the game loop, allowing developers to create, manage, and extend timers with ease.

---

### Features

- **Extensible Design**: Create custom timers by extending the `Timer` abstract class.
- **Self-Managing Timers**: Timers automatically register with a Timer Manager, which updates them as part of the Unity Player Loop.
- **High Performance**: Minimal overhead with an optimized approach to update timers during the game's lifecycle.
- **Player Loop Integration**: Timers are hooked directly into Unity's `Update` loop for precise timing.

---

### Installation

1. Download the package or include it via the Unity Package Manager.
2. Ensure the package is located at `Packages/Player Loop Timers` in your Unity project.
3. The system automatically initializes after assemblies are loaded at runtime.

---

### How It Works

1. **Timer Management**:  
   `TimerManager` acts as the central hub, managing the lifecycle of all timers. Timers register themselves with the manager and are updated during each frame.

2. **Timer**:
    The `Timer` abstract base class provides all infrastructure necessary for a Timer (inheritors)

3. **Countdown Timers**:  
   The `CountdownTimer` class inherits from the `Timer` class and provides a basic implementation of a timer that counts down from an initial value.

4**Improved Player Loop**:  
   The `TimerBootStrapper` modifies Unity's Player Loop system to include the `TimerManager`. It ensures timers are processed during the `Update` loop without manual intervention.

5**PlayerLoop Utilities**:  
   The `PlayerLoopUtils` class, a utility from another package, is utilized to manage Player Loop modifications seamlessly.

---

### Usage Example

#### Creating a Timer:
```csharp
using PsigenVision.ImprovedTimers;

CountdownTimer myTimer = new CountdownTimer(10f); // Countdown from 10 seconds
myTimer.Start();
```

#### Player Loop Integration:
The `TimerManager` is seamlessly integrated into Unity's Player Loop and updates all registered timers automatically. Developers only need to interact with timers themselves.

#### Custom Timer:
Extend the `Timer` abstract class to create your own custom timer logic.

---
# TimerBootStrapper API Documentation

**Namespace**: *`PsigenVision.ImprovedTimers`*

## Overview
The `TimerBootStrapper` class serves as an integration point for the `TimerManager` in Unity's Player Loop. It provides methods for initializing and managing the `TimerManager` subsystem, allowing timers to update automatically within the **Update Player Loop** phase.

---

## Methods

- **`void Initialize()`**  
  Initializes the `TimerBootStrapper`. This configures necessary settings or lifecycle handling required for the proper functioning of the `TimerManager` in the Player Loop.

- **`bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)`**  
  Inserts the `TimerManager` into a specific position of Unity’s `PlayerLoopSystem`.
    - **Parameters**:
        - `ref PlayerLoopSystem loop`: The Player Loop that is being modified.
        - `int index`: The index at which the `TimerManager` system should be inserted.
    - **Returns**:
        - `true` if insertion was successful, otherwise `false`.

- **`void RemoveTimerManager<T>(ref PlayerLoopSystem loop)`**  
  Removes the `TimerManager` subsystem from Unity’s `PlayerLoopSystem`.
    - **Parameters**:
        - `ref PlayerLoopSystem loop`: The Player Loop that is being modified.

---

# TimerManager API Documentation

**Namespace**: *`PsigenVision.ImprovedTimers`*

## Overview
The `TimerManager` is a centralized manager for maintaining and updating active timers within the game. It provides mechanisms to register, deregister, and update timers. The `TimerManager` is typically hooked into Unity's **Update Player Loop** system through the `TimerBootStrapper`.

---

## Fields

- **`List<Timer> timers`** *(Private)*  
  A list that stores all currently registered timers.

---

## Methods

- **`static void RegisterTimer(Timer timer)`**  
  Registers a new timer with the `TimerManager`. The registered timer will be managed and updated automatically.
    - **Parameters**:
        - `Timer timer`: The timer instance to be registered.

- **`static void DeregisterTimer(Timer timer)`**  
  Deregisters a timer from the `TimerManager`.
    - **Parameters**:
        - `Timer timer`: The timer instance to be removed.

- **`static void UpdateTimers()`**  
  Updates all registered timers by invoking their `Update()` method. This operation mimics a ticking mechanism and is typically triggered within each frame when integrated into the Player Loop.

- **`static void Clear()`**  
  Clears all registered timers from the `TimerManager`.


---


# Timer API Documentation

**Namespace**: *`PsigenVision.ImprovedTimers`*

## Overview
The `Timer` class serves as a base class for implementing time-tracking mechanisms in Unity. It provides core functionality such as starting, pausing, resuming, stopping, resetting, and updating a timer over time.

---

## Properties

- **`float CurrentTime`**  
  The current time remaining for the timer.

- **`bool IsRunning`**  
  Indicates if the timer is currently running.

- **`float Progress`**  
  Represents the progress of the timer. (For example, this could be the percentage of elapsed time.)

- **`bool IsFinished`**  
  Determines whether the timer has finished its operation, i.e., the countdown is complete or the set time has elapsed.

---

## Callbacks/Events

- **`event Action OnTimerStart`**  
  A callback fired when a timer is started (Start method called)

- **`event Action OnTimerStop`**  
  A callback fired when a timer is stopped and not finished (Stop method called)

- **`event Action OnTimerResume`**  
  A callback fired when a timer is resumed (Resume method called when a timer has already been started and isn't running)

- **`event Action OnTimerPause`**  
  A callback fired when a timer is paused (Pause method called when a timer has already been started and is running)

- **`event Action OnTimerFinish`**
  A callback fired when a timer has finished (inheritors should call the protected Stop method with a finished parameter of true at the point the timer finishes)

---

## Constructors

- **`Timer(float initialTime)`**  
  Initializes a `Timer` instance with the specified initial time in seconds.

---

## Methods

- **`public void Start()`**  
  Starts the timer.

- **`public void Stop()`**  
  Stops the timer (before it is finished).

- **`public void Stop()`**  
  Stops the timer.

- **`public void Resume()`**  
  Resumes the timer if it was paused.

- **`public void Pause()`**  
  Pauses the timer without resetting its current time.

- **`public void Reset()`**  
  Resets the timer to its initial time.

- **`public void Reset(float newTime)`**  
  Resets the timer and sets a new initial time in seconds.

- **`public void Update()`**  
  Updates the state of the timer, typically called within a game loop or update mechanism.

- **`public void Dispose()`**  
  Releases resources used by the timer.

- **`protected void Dispose(bool disposing)`**  
  Releases resources with additional disposal configuration.

- **`protected void Stop(bool finished)`**  
  Performs all stopping steps (deregistration of timer, change of internal state) for the timer, and invokes the correct event based on the finished parameter (OnTimerFinish for finished being true, and OnTimerStop otherwise)

---

# CountdownTimer API Documentation

**Namespace**: *`PsigenVision.ImprovedTimers`*  
**Inheritance**: `CountdownTimer` → `Timer`

## Overview
The `CountdownTimer` class is a specialized implementation of the `Timer` class. It counts down from a specified time and stops when the countdown reaches zero or a negative value. Ideal for use cases such as countdowns for cooldowns, delays, or other time-based gameplay mechanics.

This class operates by decrementing the current time value each frame while the timer is running.

---

## Constructors

- **`CountdownTimer(float initialTime)`**  
  Initializes a `CountdownTimer` with a specified duration in seconds.

---

## Properties

- **`bool IsFinished`**  
  Determines whether the countdown has been completed. Overrides the `IsFinished` property from the `Timer` base class.
    - **Behavior**: Returns `true` if `CurrentTime` is greater than zero, otherwise `false`.

---

## Methods

- **`void Update()`**  
  Updates the countdown timer's state and performs the following operations:
    - If the timer is not running, the method exits without updating the timer.
    - If the timer is running and `CurrentTime > 0`, it decrements `CurrentTime` using `Time.deltaTime`.
    - If the timer reaches zero or a negative value, the timer automatically stops itself.

  **Example Pseudocode**:
  ```pseudo
  if IsRunning is false:
      exit
  if CurrentTime > 0:
      decrement CurrentTime by Time.deltaTime
  else:
      stop the timer in finished state (call Stop(true) to invoke necessary callback)
  ```

---

## Example Usage

### Basic Initialization and Use
```csharp
using UnityEngine;
using UnityEngine.UIElements;

namespace PsigenVision.ImprovedTimers.Demo
{
    public class TimerExample : MonoBehaviour
    {
        [SerializeField] private UIDocument radialBarUI;
        [SerializeField] private float timer1Duration = 10f;
        [SerializeField] private float timer2Duration = 5f;
        
        private CountdownTimer timer1, timer2;
        [SerializeField] private float outerRadialBarProgress;
        [SerializeField] private float innerRadialBarProgress;
        private int outerRadialBarSegmentCount;
        private bool isInitialized = false;
        void Awake() => Initialize();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!isInitialized) return;
            
            //Start countdown timers
            timer1.Start();
            timer2.Start();
        }

        private void Initialize()
        {
            isInitialized = true;
            
            //Bind progress of radial bars to this script as its data source
            //NOTE: From within UI builder, bind the progress property to the Progress property of the timers (e.g. timer1.Progress) - See included Sample for example
            VisualElement root = radialBarUI.rootVisualElement;
            var firstRadialBar = root.Q<RadialBar>();
            var secondRadialBar = root.Q<VisualElement>("RadialBar").Q<RadialBar>();

            if (firstRadialBar == null)
            {
                Debug.LogError("Outer RadialBar element not found");
                isInitialized = false;
            }

            if (secondRadialBar == null)
            {
                Debug.LogError("Inner RadialBar element not found");
                isInitialized = false;
            }

            if (isInitialized)
            {
                firstRadialBar.dataSource = this;
                secondRadialBar.dataSource = this;
            }
            
            //Create countdown timers
            if ((timer1 = new CountdownTimer(timer1Duration)) == null)
            {
                Debug.LogError("Failed to create timer1");
                isInitialized = false;
            }

            if ((timer2 = new CountdownTimer(timer2Duration)) == null)
            {
                Debug.LogError("Failed to create timer2");
                isInitialized = false;
            }

            if (!isInitialized) Debug.LogError("Failed to initialize timer example");
            else
            {
                //Calculate the amount of segments in outer radial bar
                outerRadialBarSegmentCount = Mathf.CeilToInt(timer1Duration); //Derive how many segments we want in the bar
                
                //Provide debug log message hooks into timer start and finish callback events
                timer1.OnTimerStart += () => Debug.Log("Timer1 started");
                timer2.OnTimerStart += () => Debug.Log("Timer2 started");
                timer1.OnTimerFinish += () => Debug.Log("Timer1 finished");
                timer2.OnTimerFinish += () => Debug.Log("Timer2 finished");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isInitialized) return;
            //Update radial bars' progress with the countdown timers' progress
            //First radial bar will be treated as a segmented radial bar
            //Derive how many segments are currently visible based on the timer's progress by dividing by the total segment count
            outerRadialBarProgress = 
                (float)Mathf.CeilToInt(timer1.Progress * outerRadialBarSegmentCount) //Calculate the amount of segments to show
                / outerRadialBarSegmentCount //Convert to normalized progress
                * 100f; //Convert to percent (the units used by the radial bar)
            
            //Second radial bar will be treated as if it is continuous
            innerRadialBarProgress = timer2.Progress * 100f;
        }

        void OnDestroy()
        {
            //Dispose of our timers (perform cleanup) when the object is destroyed
            timer1.Dispose();
            timer2.Dispose();
        }
    }
}

```

---



### External API Documentation

#### `PlayerLoopUtils` (From PsigenVision Utilities Toolkit)
- **Purpose**: Facilitates Player Loop modifications, making it easier to insert and remove subsystems from Unity's Player Loop structure. This is crucial for ensuring that the `TimerManager` is integrated cleanly into the game loop.

- **Key Methods**:
    - `InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)`: Inserts a new subsystem (`systemToInsert`) at a specific `index` within the Player Loop system.
    - `RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)`: Removes an existing subsystem (`systemToRemove`) within the Player Loop system.
    - `HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)`: Helper method to handle system insertion in the Player Loop.
    - `HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)`: Helper method to handle system removal from the Player Loop.
    - `PrintPlayerLoop(PlayerLoopSystem rootLoop)`: Prints the entire Player Loop structure for debugging purposes.
    - `PrintPlayerLoopSystem(PlayerLoopSystem loop)`: Prints a specific Player Loop system for detailed inspection.

---

### Requirements

- **Unity Version**: Compatible with Unity 2020.3 and above.
- **Framework**: `.NETFramework,Version=v4.7.1`.

---

### Support and Contributions

For support or to contribute to this package, please contact the developer or submit a pull request.
