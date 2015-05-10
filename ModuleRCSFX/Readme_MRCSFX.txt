ModuleRCSFX is a fixed version of the stock RCS module.
It is derived from ModuleRCSFX by ialdabaoth (who is awesome).
It supports a lot of configuration, as well as fixing stock bugs.

** RCS Part Controls **
useZaxis defaults to false. If you set it to true, the RCS will fire along the Z axis of the given transform(s). This means you can use engine part models as RCS parts (like using the ion engine model as an RCS part).

** RCS Axis Control **
enablePitch
enableYaw
enableRoll
enableX
enableY
enableZ
All these default to true, but if one is set to false in the MODULE, the RCS part will not fire for that input. These can be toggled in the VAB/SPH.

useThrottle
which, when set to true, means that RCS will fire forwards with the throttle.


** RCS Thrust Control **
fullThrust defaults to false. Set it to true and RCS will always fire at full thrust (or 10% thrust in precision mode), rather than the less-than-full-thrust, dependent-on-angle they do stock.

useLever defaults to false. When it's false, fine controls will make RCS fire at 10% (default) power only. When it's set to true, stock behavior returns (i.e. fine controls means lever arm compensation).

precisionFactor is the multiplier to use when useLever is false (as it is by default). precisionFactor defaults to 0.1 (10%).

** RCS Input Controls **
EPSILON defaults to 0.05. That means a control actuation of less than 5% is ignored. This is because Unity is bad at joysticks and ignores deadzones.

** RCS Effects **
Currently disabled pending rework.


INSTALLATION:
unzip to GameData

LICENSE remains the ialdabaoth license (CC-BY-SA + tweaks).
SOURCE is https://github.com/NathanKell/ModuleRCSFX

CHANGELOG
v4.0
* Update to KSP 1.0.
* Speed improvements. (Death to foreach! Don't recalculate values!)
* Removed EFFECTS support for now (doesn't work).
* Easy integration with TestFlight.
* Added useLever, tunable precision mode thrust, tweaked EPSILON support.

v3.5
*Fix enable/disable functionality.
*Fix non-PROPELLANT RCS.
*Change how thrust scaling works: now thrust is scaled by thrusterPower correctly (I trust), and precision mode is always "10% thrust" rather than varying based on placement.
*Made rotatation/linear restrictions toggleable in the VAB.

v3.4
*Add control clamping
*Update to 0.90

v3.3
*Remove debug spam

v3.2
*Fix more stock RCS bugs (no longer capped at min 100N thrust).

v3.1
*Recompile for KSP 0.25

v3.0
*Fixed issues below 750m/s (thanks chicknblender!)
*Added EFFECTS support back in
*Added fullThrust as an option.

v2.x
*Fixed for .24
*Added the new fields
*Thrust correction by default.