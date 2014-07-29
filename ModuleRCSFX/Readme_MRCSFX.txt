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

FX attributes, which did more harm than good, have been stripped for now.

correctThrust defaults to true. Set it to false if you want stock KSP behavior regarding Isp and fuel flow.

v0.4

INSTALLATION:
unzip to GameData

License remains the ialdabaoth license (CC-BY-SA + tweaks).
Source is https://github.com/NathanKell/ModuleRCSFX