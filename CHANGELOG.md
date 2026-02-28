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