# Timer Utility

`Timer` primarly functions as a timer and can also be used to lerp values if you want to. It utilizes the observer pattern intensively for easily managing behaviours.

## Features

- **Flexible Update Types:** Choose between Update, FixedUpdate, LateUpdate, and even custom time intervals for your `Timer`s update method with Manual updates.
- **Event System:** Set up event callbacks for `OnStart`, `OnUpdate`, `OnComplete`, `OnStop`, `OnKill`, `OnPause`, and `OnUnpause`.
- **Filtering:** Access and filter through all your timers easily with the `TimerRunner.AliveTimers` list and `Name` property on the `Timer`s.
- **Tweening:** You can tween/lerp values with provided methods in the `TimerUtilities.EasingFunctions` class.
- **Efficient:** The utility is designed with performance and memory in mind; it does not use coroutines internally.
- **Fully Documented:** All code you can access is XML documented clearly and accurately.
- **Open Source:** The code is open source.

## Setup

1. Get the scripts inside your project, ideally to the `Assets/Plugins/SametHope/TimerUtility` folder.
2. Done.

## Usage

Here are some example code snippets:

```csharp
// Create
Timer timer = new Timer();

// Setup
timer.Name = "MyTimer";
timer.UpdateMethod = TimerUpdateMethod.FixedUpdate; // Default is TimerUpdateMethod.NormalUpdate
timer.CompleteTime = 10f; // If not set, the timer will run forever until it is manually completed/stopped/killed

// Subscribe to events you need
timer.OnStart += () => Debug.Log("Started.");
timer.OnUpdate += () => Debug.Log("Elapsed.");
timer.OnComplete += () => Debug.Log("Completed.");
timer.OnStop += () => Debug.Log("Stopped.");
timer.OnKill += () => Debug.Log("Killed.");
timer.OnPause += () => Debug.Log("Paused.");
timer.OnUnpause += () => Debug.Log("Unpaused.");

// Start
timer.Start();

// Access globally
bool isTimerPaused = TimerRunner.AliveTimers.Find(t => t.Name == "MyTimer")?.IsPaused ?? false;

// Pause and unpause
timer.Pause();
timer.Unpause();

// Complete manually
timer.Complete(updateElapsedTime: false);

// Stop manually
timer.Stop();

// Kill manually
timer.Kill();

// Tween
Vector3 startPosition = transform.position;
Vector3 endPosition = transform.position + Vector3.up;
float easeValue = TimerUtilities.EasingFunctions.InOutSine(timer.GetNormalizedTime()); // Use an easing function to get an interpolated value based on the normalized time
Vector3 interpolatedPosition = Vector3.Lerp(startPosition, endPosition, easeValue);

// Utility
bool notStartedYet1 = timer.IsAliveAndNotNull(); // Extension call for ease of use. Normal IsAlive property is obviously not accessible on null instances unlike this method

// Furthermore
[SerializeField] private Timer _myTimer; // Serialize a timer to show it in the inspector, really useful for debugging. You can also just make it public too, of course.
```