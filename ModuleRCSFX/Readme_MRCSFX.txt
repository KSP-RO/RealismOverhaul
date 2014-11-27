ModuleRCSFX is a fixed version of the stock RCS module.
It is derived from ModuleRCSFX by ialdabaoth (who is awesome).
It also adds support for fields
enablePitch
enableYaw
enableRoll
enableX
enableY
enableZ

All these default to true, but if one is set to false in the MODULE, the RCS part will not fire for that input.
Also there is
useThrottle
which, when set to true, means that RCS will fire forwards with the throttle.

correctThrust defaults to true. Set it to false if you want stock KSP behavior regarding Isp and fuel flow.

fullThrust defaults to false. Set it to true and RCS will always fire at full thrust (or 10% thrust in precision mode), rather than the less-than-full-thrust, dependent-on-angle they do stock.

You may either use the default RCS effects, or add an EFFECTS node in your part cfg and in the MODULE assign:
runningEffectName
engageEffectName
flameoutEffectName


INSTALLATION:
unzip to GameData

LICENSE remains the ialdabaoth license (CC-BY-SA + tweaks).
SOURCE is https://github.com/NathanKell/ModuleRCSFX

CHANGELOG
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