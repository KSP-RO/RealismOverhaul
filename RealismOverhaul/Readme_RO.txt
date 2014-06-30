This mod is maintained by RedAV8R and NathanKell (with contributions from many)

License CC-BY-SA

Also included: Module Manager (by sarbian, swamp_ig, and ialdabaoth). See Module Manager thread for details and license and source: http://forum.kerbalspaceprogram.com/threads/55219
Module Manager is required for RO to work.

INSTALL:
1. Install required mods, updating as necessary. See the Requires list below for installation notes for those mods.
2. Remove any existing RealismOverhaul or RedAV8R or SFJBRealEngines folder in KSP/GameData/
3. Unzip this archive to KSP/GameData.
========================================

This is a set of ModuleManager tweaks and part rescales for a 100% human-scale KSP.

Requires:
Real Solar System (else why use this?)
Real Fuels v6.2+
Procedural Parts (or you won't have the right size tanks)
Deadly Reentry v4.6+ (for the heatshields)
Ferram Aerospace Research by the amazing ferram4
RealChutes v1.1+ by stupid_chris
Engine Ignitor by HoneyFox. NOTE: DO NOT extract the included zips of config files; if you already extracted the config files, delete them.
ModuleRCSFX by ialdabaoth
Advanced Jet Engine by camlost
Engine Thrust Controller by HoneyFox. NOTE: DELETE the cfg that comes with it.
KM_Gimbal. If you already have Space Shuttle Engines, you have this. Otherwise you need it.

Best with:
Medieval Nerd's Realistic Tech Progression Lite tech tree (note: requires RftSEngines, not RealEngines)
RemoteTech 2
Procedural Fairings
TAC Life Support
Supported parts packs
Realism patches for those packs (see post 2 in the Realism Overhaul thread)
ModuleFixer

Supported parts packs:
AIES (Partial support for pods, full support for battery/RTG/solar panels, RCS)
KW Rocketry (batteries, fairings, RCS)
NovaPunch (partially, more support coming soon)

KOSMOS Salyut parts
RLA RCS
ALCOR
B9 (partial)
Fustek Parts Expansion
NFPP Solar Panels
PorkWorks Inflatable Habs
FASA v4.91 - Latest Release
Bobcat Soviet engines pack (Soviet/Russian engines)
Lovad Saturn, HIIA/B
KerbX
LazTek - Latest Release
Taurus HCV
SDHI
Aerojet Kerbodyne

Engine Support:
Realism Overhaul comes with the RealEngines set of engine patches. It gives engines the stats of real-world engines that look similar, like NovaPunch K2-X -> J-2. However, it also supports NathanKell's Reaching for the Stars pack, which gives engines "realistic" stats informed by real-world engines without being clones. Note that in either case you need KM_Gimbal for gimbals to work.
My RftS Pack can be downloaded from the fourth post here: http://forum.kerbalspaceprogram.com/threads/57833

Changes:
*New size system: 0.5m, 1m, 2m, 3m, etc.
*Rescales of stock parts and Procedural Fairings to support the new size system.
*Realistic battery and solar panel numbers, and usage for RT2 antennae. 1EC/s = 1kW. Solar panels don't produce much, but they're light.
*RTGs are now modeled after real RTGs.
*DRE Heatshields support RSS, new heatshields added
*Various structural/aero parts had their masses tweaked, including PF large bases. PF lage bases now have tiny decouplers.

Partial support:
Pods: Squad Mk1, Mk1-2, AIES Orbital Pod.
Probes: the Squad probes. AIES probes have electrical charge fixed but neither rescaled nor remassed as yet.
Note that only the two stack probes (now 2m and 3m) have much in the way of torque; even then you'll want to bring RCS. Everything else doesn't have any torque to speak of.

Changelog:
v5.2 \/
*Converted to TAC Life Support! Now includes TACLS settings file, to overwrite existing TACLS settings for realistic consumption rates for all resources. Your window positions will reset.
*Upgraded to Module Manager 2.1.5
*ThorBeorn: Add support for the new version of FusTek
*Fix RT2 patch again.
*Update ALCOR pod name
*Removed landing gear and control surface rescales for B9
*Support Proc Fairings 3.0
*RedAV8R patch updates:
+FASA Realism Patch update to correspond with v4.91
+OLDD Realism Patch Added (Saturn only) – (Proton and N1 TBD)
+KerbX Realism Patch Added
+LazTek Realism Patch Added (supports Launch Pack 3.0, Exploration 2.0, Historic 1.0)
+AerojetKerbodyne/SDHI Service Module/Taurus HCV Realism Patch Added
+TAC Life Support support for all Realism Patches
+RealEngines now included – Klockheed Martian SSME (OMS coming soon), BobCat Soviet Engines, KOSMOS Engines, Hakari F-1, more coming soon (like SRBs and LES). RftS is optional, no action needed for use.
+All Realism Patches with new ModuleManager ‘:FOR[RealismOverhaul]’ tag added
+Coresponding changes/removal of duplicate/unneeded/unwanted configurations

**NOTE** Delete your old SFJBRealEngines folder if you have one!

v5.1  \/
*Updates to FASA patches by RedAV8R
*Fixed heatshield tangents not being set, leading to unexpected behavior
*Add patch for MissionController (patches fuels and engine costs to be 1k = $1000 USD 1960)
*Update RT2 patch file, fix errors
*Add throttling and EngineIgnitor support to BobCat Soviet Engines
*Upgrade to ModuleManager 2.1.0

v5 -- \/
*Moved WAC Corporal and V-2 parts to RPL; added in Engine Ignitor and throttle-limiting support
*Merged RedAV8R's FASA patches (maintained by RedAV8R)
*Merged brooklyn666's RT2 patches
*Updated 2.5m heatshield title for clarity
*Fix Proc Fairing node sizes
*Switched to ModuleRCSFX for RCS thrusters. Now supports bipropellant RCS!
*Updated RealChute integration with new parts (thanks stupid_chris!)
*Will automatically set correct FAR settings (0.13.2+)

v4 -- \/
*Fix typo for RTG, Stayputnik, WAC probe core, AIES
*Add missing KW RCS
*Remove FASA config (handled by RedAV8R)
*Removed inbuilt support for RealEngines
*Changed scaling method for likely-to-be-root parts
*Add community rescales (broman [Porkworks habs], amo28 [KOSMOS Salyut], Phredward [10m heatshield, docking ports])

v3 -- \/
*Switched to SFJackBauer's RealEngines. Added partial B9, CSS, and ISS support from that pack.
*Sum Dum Heavy Industries Service Module support
*Added modular RCS support to KW, AIES, and RLA
*KW battery support
*Added rescaled KW fairings with realistic masses
*Finished the Squad pods (lander cans, cupola)
*Added by redde: Fustek station parts rescaling (for 0.03.5a)
*Added by jrandom: realchutes patch, ALCOR rescale, Nothke's Service Components rescale, additional fairing rings.
*Added by amo28: NFPP solar panel support
*Added by SRFirefox: Hitchhiker and Lab rescale
*Added by dlrk: 2M Clamp-O-Tron and Shielded Clamp-O-Tron

v2 -- \/
*Included missing 0.625m heatshield fix for DRE
*Changed Solid rockets as well.
*Merged with my new textures (and parts) for Procedural Fairings. NOTE: FOLLOW INSTALL INSTRUCTIONS ABOVE EVEN IF UPGRADING.

v1 -- \/
Initial Release.