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

2. **Countdown Timers**:  
   The `CountdownTimer` class provides a basic implementation of a timer that counts down from an initial value.

3. **Improved Player Loop**:  
   The `TimerBootStrapper` modifies Unity's Player Loop system to include the `TimerManager`. It ensures timers are processed during the `Update` loop without manual intervention.

4. **PlayerLoop Utilities**:  
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

### API Documentation

#### `CountdownTimer`
- **Purpose**: A timer that decrements over time until completion.
- **Methods**:
    - `Update()`: Handles the decrement of time during updates.
    - `IsFinished`: Indicates whether the timer has completed.

#### `TimerManager`
- **Purpose**: Centralized manager for registering and updating timers.
- **Methods**:
    - `RegisterTimer(Timer timer)`: Registers a timer for updates.
    - `DeregisterTimer(Timer timer)`: Unregisters a timer.
    - `UpdateTimers()`: Updates all registered timers.
    - `Clear()`: Clears all timers, used during cleanup.

#### `TimerBootStrapper`
- **Purpose**: Integrates the `TimerManager` into Unity's `Update` Player Loop.
- **Initialization**: Automatically initializes on runtime with `RuntimeInitializeOnLoadMethod`.

#### `PlayerLoopUtils` (From Utility Toolkit)
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

---

### License

[Insert your license details here, if applicable.]