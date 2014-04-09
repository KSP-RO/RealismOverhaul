This mod is maintained by NathanKell with contributions from many

License CC-BY-SA

Also included: Module Manager (by sarbian, based on ialdabaoth's work). See Module Manager thread for details and license and source: http://http://forum.kerbalspaceprogram.com/threads/55219
Module Manager is required for RO to work.

INSTALL:
1. Install required mods, updating as necessary
2. Remove any existing RealismOverhaul folder in KSP/GameData/
3. Unzip this archive to KSP/GameData.
4. Open GameData and open the FerramAerospaceResearch folder, then Plugins, then PluginData, then FerramAerospaceResearch, open config.xml in Notepad (or any text editor), find the attachNodeDiameterFactor line and change 1.25 to 1.0
5. Pick and install an engine configs pack (see below)
========================================

This is a set of ModuleManager tweaks and part rescales for a 100% human-scale KSP.

Requires:
Real Solar System (else why use this?)
Real Fuels v5.1+
Stretchy SRB v9+ OR Procedural Parts (or you won't have the right size tanks)
Deadly Reentry v4.6+ (for the heatshields)
Ferram Aerospace Research (made by the amazing ferram4)
RealChutes (by stupid_chris)
Engine Ignitor (by HoneyFox)

Best with:
Medieval Nerd's Realistic Tech Progression Lite tech tree (note: requires RftSEngines, not RealEngines)
RemoteTech 2 and brooklyn666's antenna ranges (see post 2 in the Realism Overhaul thread)
Procedural Fairings
Supported parts packs
Realism patches for those packs (see post 2 in the Realism Overhaul thread)

Supported parts packs:
AIES (Partial support for pods, full support for battery/RTG/solar panels, RCS)
KW Rocketry (batteries, fairings, RCS)
NovaPunch (partially, more support coming soon)
Bobcat Soviet engines pack (Soviet/Russian engines)
KOSMOS Salyut parts
RLA RCS
ALCOR
B9 (partial)
SDHI
Fustek Parts Expansion
NFPP Solar Panels
PorkWorks Inflatable Habs

Engine Support:
Realism Overhaul requires the selection of an Engine pack. The two suggested packs (pick one) are either SFJackBauer's RealEngines pack (which gives engines the stats of real-world engines that look similar, like NovaPunch K2-X -> J-2) and my Reaching for the Stars pack, which gives engines "realistic" stats informed by real-world engines without being clones.
SFJackBauer's RealEngines pack can be downloaded from http://forum.kerbalspaceprogram.com/threads/59207-0-22-Realism-Overhaul-ROv2-Modlist-for-RSS?p=825469&viewfull=1#post825469
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