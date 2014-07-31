Realism Overhaul

WARNING!!! THIS COULD AND PROBABLY WILL BREAK SAVES. THIS IS A WIP ALPHA RELEASE. THERE WILL BE BUGS. PLEASE REPORT.
========
This mod is maintained by RedAV8R and NathanKell (with contributions from many).

License CC BY-SA

Also included: Module Manager (by sarbian, swamp_ig, and ialdabaoth). See Module Manager thread for details and license and source.
Module Manager is required for RO to work.

FAQ:

Q: What is Realism Overhaul (RO)?
A: Realism Overhaul is a set of ModuleManager patches which transforms KSP parts (both stock and mods) into 1:1 replicas or as close to what real world would be. While totally separate, (although created by the same person - CHEERS to NathanKell), RO and RealSolarSystem (RSS) are complimentary. Neither 'requires' the other for KSP to run, but your experience is going to be severely and awkwardly changed without them together. Note that compared to a stock Kerbin universe your experience will be totally changed compared to that of what RSS and RO provides. These two combined effectively turn the 'game' of KSP, into a space simulator in which you can design and fly real missions using real numbers.

Q: What should I know about Realism Overhaul?
A: There is a lot. Whatever you have 'learned' about space travel using a stock Kerbin universe, forget it. KSP was designed around 'playability', and making things 'fun'. RSS and RO when used together makes space travel as hard as it gets. Gone are the days of lifting off straight up until your wanted apoapsis is reached then waiting, turning 90 and powering until orbital velocity is achieved...well, you can do that still, but it's time you learn more efficient flight paths or your vehicles are going to be quite large.

Changes:
*New generic size system: 0.5m, 1m, 1.25m, 1.5m, 2m, 2.5m, 3m, 3.5m, 3.75m, 4m, 5m, 6m, 7m, 8m, 9m, 10m - This is achieved via TweakScale.
*Modified ElectricCharge system. 1EC/s = 1kW.
*Realistic battery and solar panel numbers, and usage for RT2 antennae. Solar panels don't produce much, but they're light.
*RTGs are now modeled after real counterparts.
*DRE Heat shields now support RSS, with new heat shields added based on new size system
*Stock/Generic pack masses have been tweaked for realism.
*Realistic use of Reaction Wheels. Very few parts have them, and those that do have realistic forces. Most vessels will require some form of Reaction/Attitude Control System.

Engine Support:
Realism Overhaul comes with the RealEngines set of engine patches (originally by SFJackBauer). It gives engines the stats of real-world engines that look similar, like NovaPunch K2-X -> J-2.

Realism Overhaul natively supports NathanKell's Reaching for the Stars pack, which gives engines "realistic" stats with a real-world flavor without being clones. This is required when using Realistic Tech Progression Lite (RPL).
The RftS Pack can be downloaded from the fourth post here: http://forum.kerbalspaceprogram.com/threads/57833.

Realism Overhaul also natively will not interfere with "StockAlike" engine configurations.

Q: What do I need to run Realism Overhaul?
A: Realism Overhaul uses a set of 3rd party mods to enable certain aspects of realistic flight/performance/etc. Without these, 'realism' is going to be severely limited. Memory footprint (except RSS) is quite small so having them (you likely might already) is not going to produce a performance hit. Therefore, each one of these mods listed as REQUIRED are just that and support without them will not be received.

Q: I don't want to install ****** (some required mod), what will happen, what should I do?
A: Install it anyway. Realism Overhaul+RealSolarSystem is designed around these dependencies. Generally KSP will still load, but you have now nerfed the entire experience, and have shown you do not wish to realistically experience spaceflight. Again, these are REQUIRED, and support without them will not be received.

Q: My Realism Overhaul installation is broken/doesn't work/etc. What do I do?
A: First, go through these basic FAQ and see if your answer is there. If it is, great, problem solved. If not, follow the prescribed basic troubleshooting steps following the FAQ. If that doesn't fix it, THEN you may post. Please follow the directions for your post, otherwise be prepared to be chastised.

Q: My engines don't throttle! How can I fix this?
A: You don't, and we won't. It's working as designed. In real life, 99% of rocket engines don't throttle at all. A few throttle down to about 60-70% (like the SSME) to lower crew G forces on launch and to limit stress on the stack (especially at Max Q). Only a tiny, tiny few actually "deeply" throttle--basically just engines designed for landers. The Lunar Module Descent Engine is the largest deeply-throttling engine in RO right now, and will throttle down to about 12%. As more and more engines are added, your options will increase, but again, there aren't many engines that do throttle. Such is life. Welcome to Realism Overhaul.

Q: My parts are bigger/smaller than they were?
A: Yep, they sure are, and no, we are not going to change it back. Parts have been resized to approximate or duplicate real life. Such as this is Realism Overhaul, that is kind of the point.

Q: Some parts have CoM/CG offset from the true center of the part? AND/OR My craft won't fly straight, what is happening?
A: Some parts (even more will be soon) have had their CoM adjusted to approximate real life. Once again, no, we aren't going to change it back. Soon we will add a moving CoM to further enhance realism. Remember, this Realism Overhaul not Kinda Sorta Maybe Real But Totally Fiction When Things Get Tough Overhaul.

CoM Related:
Q: How do I make a controlled, lifting reentry with the Mk1-2 Pod?
A: See this awesome video for a tutorial.

Q: My engines are disappearing into my fueltank!
A: The "going into the fueltank" thing is a trick SFJackBauer devised to get around the issue that most KSP engine models are actually engines + giant fuel tank bottom domes, basically. (Look up pictures of rocket engines. At most they have a turbopump etc at the top, or a set-of-girders thrust structure. Not a giant dome; that's the bottom of the fuel or oxidizer capsule-shaped tank.) Which is bad if you, for instance, want to use the engine on a different-sized fuel tank. Can't do anything about using an engine with a giant dome on a *smaller* tank, but for bigger tanks SFJackBauer devised this system where the unsightly dome is hidden inside the fueltank. Works great for clusters like S-II.

TROUBLESHOOTING:
So, none of the previous answers satisfied you or you still have problems.

1. RE-INSTALL. Start from scratch again.

a. Install a new fresh copy of KSP. A lot of times, starting over is THE best way to remove all doubt as to what is causing an issue, ensuring there isn't anything extra that may not be supported or wanted.
b. Ensure you have the latest version of Realism Overall and all dependencies
c. Install all dependencies, following individual included instructions except for any special instructions found here. (See the Install Procedure)
d. Install the latest copy of Realism Overhaul. There may be a good chance that a new version has been released, sometimes they are daily, just depends on how much was done. If that doesn't solve the issue, you may have a bug.

2. RE-TEST. Does what was happening still happen??? Do this 2 or 3 times at least to verify.

3. GET YOUR LOGS. During your testing if the issue happens make sure you get the log. This is vital. No log, no support.

Cause the problem. Quit KSP (if it hasn't crashed). Upload your output log (NOT ksp.log) to dropbox or something.
Windows: KSP_win\KSP_Data\output_log.txt OR KSP_win64\KSP_x64_DATA\output_log.txt (depending on which used)
Mac OSX: Open Console, on the left side of the window there is a menu that says 'files'. Scroll down the list and find the Unity drop down, under Unity there will be Player.log Aka Files>~/Library/Logs>Unity>Player.log
Linux: ~/.config/unity3d/Squad/Kerbal\ Space\ Program/Player.log

4. POST. Now that you've got your 'evidence', now is the time to let us know what is going on.

a. State your issue.
b. State how you got your issue to occur. The more detail in these two steps the better it helps us to narrow down specific events.
c. Post a link to your log that you obtained in STEP 3.

5. WAIT FOR RESPONSE. Somebody should answer your question pretty quick, but it may be a day or two.

6. READ, UNDERSTAND, FOLLOW DIRECTIONS PRECISELY. Sometimes we need more information, have a request for you to try, or advice. If we do, do what we say.

Now the Fun STUFF.

INSTALLATION:
1. Start with a FRESH unadulterated install of KSP.
2. Install the required mods. Ensure you have the most up to date version and have followed their respective installation instructions. Links are provided below as well as some special installation notes/instructions for those mods.
3. Unzip the Realism Overhaul download zip archive to your KSP/GameData folder.
4. OPTIONAL - Those using PartCatalog may CUT and paste the folders within RealismOverhaul/RedAV8R into your KSP/GameData, there should be no overwriting necessary.
5. Launch KSP. Enjoy.
========================================

Requires:
Module Manager by sarbian, swamp_ig, and ialdabaoth - INCLUDED (v2.2.0)
ModuleRCSFX by ialdabaoth. Updated by NathanKell - INCLUDED
Real Solar System by NathanKell (v7.1)
Real Fuels by NathanKell (v7.1)
Deadly Reentry Continued by NathanKell (v5.2)
Ferram Aerospace Research by ferram4 (v0.14.1.1)
RealChutes by stupid_chris. (v1.2.3) NOTE: DO NOT install the MM files that are included.
Engine Ignitor by HoneyFox. (v3.3) NOTE: DO NOT install the MM files that are included.
Advanced Jet Engine by camlost (v1.4) NON-OFFICIAL 0.24.* update HERE

Very Highly Recommended:
TweakScale by Biotronic (v1.33)
TAC Life Support by TaranisElsu (v0.9 PRE-RELEASE #3)
CrossFeedEnabler by NathanKell (v2.2)
Toolbar by blizzy78 (v1.7.6)
ModuleFixer by arsenic87
PartCatalog by BlackNecro (v3.0RC5)
TextureReplacer by Shaw (v1.6.1)
or Active Texture Management by rbray89 (v3.3.1)
MechJeb by many (v2.3.1)
Kerbal Joint Reinforcement (v2.4.3)
RemoteTech 2 by Remote Technologies Group (v1.4)

Mods/Part Packs:
Procedural Parts by swamp_ig (v0.9.16) (FULL SUPPORT)
Procedural Fairings by e-dog (v3.08) (FULL SUPPORT)
KW Rocketry by Winston & Kickasskyle (v2.6c) (FULL SUPPORT)
Procedural Dynamics - Procedural Wing by DYJ (v0.8) (WIP)
AIES by carmics (v1.5.1) (WIP)
NovaPunch by Tiberion (v2.05) (WIP)
KOSMOS Spacecraft Design Bureau by Normak (4.7) (WIP)
RLA (WIP)
ALCOR by (WIP)
B9 (WIP)
Fustek Station Parts (WIP)
Near Future Technologies by Nertea (v2.*) (WIP)
PorkWorks Inflatable Habs by Porkjet (WIP)
FASA by Frizzank (v4.97) (WIP)
Bobcat Soviet Engines (WIP)
World Space by Lovad (WIP)
KerbX by Borklund (WIP)
LazTek SpaceX by LazurusLuan (WIP)
Taurus HCV by jnrobinson (v1.2.1) (WIP)
Aerojet Kerbodyne (WIP)
OLDD Apollo/Saturn (WIP)
Rocketdyne F-1 (WIP)
Universal Storage by PaulKingtiger (v0.752) (WIP)

Optional:
Realistic Tech Progression Lite tech tree by Medieval Nerd (note: requires RftSEngines)

Coming Soon:
Sum Dum Heavy Industries - Service Module System by sumghai (v2.0) (NOT SUPPORTED YET WILL BE SOON)
Lionhead ESA, Rovers, Landers, Probes, Solar Panels by 
Component Space Shuttle maintained by Dragon01
USI Kolonization Systems (MKS/OKS) by RoverDude
Spaceplane Plus by Porkjet
Kethane by Majiir
SCANsat by technogeeky
SkyLab by raidernick
International Space Station

Changelog:

v6.0 ALPHA 8 \/
*Small batteries in squad pods
*FASA updates
*RealEngine updates to Squad, RealFuels, KW, and BobCat
*Update to ModuleRCSFX
*TACLS Config Fix
*Convert UA120X to UA1204
*Mass/Thrust/Isp Fix for fairings
*260/300" PLFs added
*PLF Base mass fix
*Better, not quite perfect thrust curves
*KW Solids complete - needs better thrust curves

v6.0 ALPHA 7 \/
*Rework DRE Heatshields, more size options available now.
*Update TAC Consumption/Production Rates
*Add new Thoikol M55 SRM
*Apollo Heatshield fix for FAR
*Add J-2 option
*Fix Generic RCS and rework with +PART
*Add back ModuleRCSFX - NOW INCLUDED
*Life Support adds to Stock Pods
*Add FASA UA1207
*Add new SRB Thrust Curves to multiple engines (still some to do)
*New TweakScale FreeScale option for structural components
*Fix TAC Tank Definitions
*ModuleManager 2.2.0 - NOW INCLUDED
*Minor Isp Fix for Aerojet SPS Engine
*New RealEngines SRBs - Castor 120/30/30XL/UA1205
*New RealEngines Liquid - SpaceX Merlins, Aestus II
*New TweakScale Nose Cone for SRBs

v6.0 ALPHA 6 (6.1/6.2 HOTFIX) \/
*Fix Typos
*Fix RealChutes

v6.0 ALPHA 6 \/
**UPDATE to KSP 0.24.* (Work In Progress folks, PLEASE report bugs, and be forgiving)
* Now using stock gimbal module - KM_Gimbal no longer required, but optional, and can be used on some parts for more realistic operation
* Now using stock RCS module - Squad made some good welcome realistic changes
* More RealEngine support added, WAY more to come!!!

v6.0 ALPHA 5 \/
***NOT ready for KSP 0.24 (this update contains a warning of that).
*Fixed MOL issues
*Added LazTek Docking Port back
*RCS pod updates
*TACLS updates
*Universal Storage updates
*KOSMOS updates

v6.0 ALPHA 4 \/
*Fixed some deprecated parts
*Porkworks Habitat Pack rework
*TACLS v.9 pre-release updates
*KOSMOS Pack work (still WIP)
*ElectricCharge update (WIP)
*Plugin - Version Checker
*ADD Universal Storage compatibility

v6.0 ALPHA 3 \/
*FusTek Complete (current with X0.04-4 DEV BUILD)
*Fix ProceduralParts LifeSupport Tank to 1U=1L standard
*Added TweakScale support to 10m for parts that use it
*Removed TweakScale with DRE (values not appropriate)
*Completed ASET ALCOR pod update
*Completed PorkWorks HabitatPack update
*Completed Nothke Service Compartment update - Added TweakScale
*NP2/KW/AIES RealEngine Updates
*Added TweakScale support to AIES Fuel Tanks

v6.0 ALPHA 2 \/
*SDHI Docking Port Fix
*Squad Inline Docking Port Fix
*NP2 RealEngine Fixes for missing engines
*Readme WIP
*More FusTek Station Parts Work, still not done, closer.

v6.0 ALPHA 1 \/
*New File/Folder Organization
*Integration of TweakScale
*Various Realism patch updates and fixes

v5.2.1 \/
*Upgrade FASA support to the latest 4.92 release

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

Download
Github

FAQ:
Q: How do I do X in Real Solar System / RO?
A: Check the RSS Wiki, it may have an answer!

Q: My engines are disappearing into my fueltank!
A: The "going into the fueltank" thing is a trick SFJackBauer devised to get around the issue that most KSP engine models are actually engines + giant fuel tank bottom domes, basically. (Look up pictures of rocket engines. At most they have a turbopump etc at the top, or a set-of-girders thrust structure. Not a giant dome; that's the bottom of the fuel or oxidizer capsule-shaped tank.) Which is bad if you, for instance, want to use the engine on a different-sized fuel tank. Can't do anything about using an engine with a giant dome on a *smaller* tank, but for bigger tanks SFJackBauer devised this system where the unsightly dome is hidden inside the fueltank. Works great for clusters like S-II.

Q: How do I make a controlled, lifting reentry with the Mk1-2 Pod?
A: See this awesome video for a tutorial.

OLD THREADS:
Old RealismOverhaul Thread
Old RedAV8R Realism Patch Thread