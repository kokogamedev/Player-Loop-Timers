# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-02-28

### This is the first release of *\<PsigenVision Player Loop Timers\>*.

### Added
- **CountdownTimer**: A basic countdown timer implementation that decrements over time.
- **TimerManager**: Centralized timer management, allowing registration, deregistration, and updating of timers.
- **TimerBootStrapper**: Integration of `TimerManager` into Unity's `Update` Player Loop for automatic timer updates.
- **PlayerLoopUtils**: Helper utilities located in a separate PsigenVision Utilities package for modifying and debugging Unity's Player Loop structure.
- Basic structure for creating custom timers by extending the `Timer` abstract class.

### Notes
- This package is inspired by and based on `git-amend`'s Improved Timers package and follows their tutorial.
- The project is in its **first release** and may not function perfectly.
- **Known Issue**:
    - The `TimerBootStrapper.Initialize` method currently faces an issue where the extracted current player loop (`PlayerLoop.GetCurrentPlayerLoop()`) is returning `null`. This requires debugging.
- The package is not yet fully functional and is a **work in progress**.

---

## [0.1.1] - 2026-02-28

### This is a fix patch for some logging that threw errors

### Fixed
- Removed issue with logging issue in the TimerBootstrapper in which the type of the cached currentPlayerLoop was being requested, which is always null

---
## [0.1.2] - 2026-03-02

### This is a small refactoring and addition of callback events.

### Added
- **OnTimerPause** and **OnTimerResume** callbacks added to Timer and all its inheritors, called by **Pause** and **Resume** methods, respectively
- **Start** and **Stop** methods now track timer start-state using flag
- **Pause** and **Resume** guarded by start-state flag (not executed if timer is not started)
- All callbacks initially assigned to null rather than an anonymous delegate for performance purposes
- All methods made virtual for maximum extensibility

---
## [0.1.3] - 2026-03-02

### A small refactoring and addition of callback events.

### Added
- **OnTimerFinish** callback added to Timer and all its inheritors, called within a newly added Stop method that accepts the boolean finished parameter
- Added **protected void Stop(bool finished)** method to perform all timer-stopping logic, and invoke the appropriate callback based on the finished parameter (OnTimerFinish for finished = true, OnTimerStop otherwise)

### Changed
- Adjust public **Stop()** method to simply call **Stop(false)**, retaining its original logic and callback invocation
- In the CountdownTimer class, call Stop(true) rather than Stop() within Update() method to ensure invocation of the OnTimerFinish callback
- Convert all delegate Action callbacks in the Timer class to Action events for safety
- Remove/Comment out any logging/printing methods left over from development stage

---
## [0.1.4] - 2026-03-02

### A small refactoring chore - renamed namespace from "ImprovedTimers" to "PlayerLoopTimers"

---
## [0.1.5] - 2026-03-13

### A small refactoring and dependency update

### Refactored
- Moved failure logging from PlayerLoopUtils.HandleSubSystemLoop to TimerBootStrapper.InsertTimeManager

### Updated Dependencies
- Updated PsigenVision.Utilities package dependency to v1.0.20

---