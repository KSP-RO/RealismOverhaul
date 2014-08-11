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
Realism Overhaul now comes integrated with the RealEngines set of engine patches (originally by SFJackBauer). It gives engines the stats of real-world engines that look similar, like NovaPunch K2-X -> J-2. No action is required to use RealEngines. For ultimate realism please stick with what comes with Realism Overhaul and wait until RPL supports RealEngines, then play in Career Mode.

Realism Overhaul natively supports NathanKell's "Reaching for the Stars" engine pack, which gives engines "realistic" stats with a real-world flavor without being clones. If you want some small Career Mode support and have installed Realistic Tech Progression Lite (RPL), then the RftS engine pack is REQUIRED. Once installed, RftS DISABLES RealEngines. The RftS Pack can be downloaded from the fourth post here: http://forum.kerbalspaceprogram.com/threads/57833. FYI, soon RPL will work with RealEngines, at that time the RftS engine pack should be removed to gain RealEngines capability back.

Realism Overhaul also natively supports "StockAlike" engine configurations. If "StockAlike" is installed then ALL RealEngines will be DISABLED. I personally do not recommend this option as it goes against everything Realism Overhaul stands for, however, the option is there.

NOT Supported at this time:
Integrated career mode. That means, no tech tree, no good cost system, contracts, the whole nine yards. It's enough work just getting things to work at all, let alone to take the time to ensure there is a good natural progression for career mode. So at this time, the only thing supported is 'sandbox' mode. If somebody else wants to take that part of the project on, please let me know. Otherwise it is the last on my list, and anything new that comes up will be placed in front of it as well. It is that low on my priorities list.

Q: What do I need to run Realism Overhaul?
A: Realism Overhaul uses a set of 3rd party mods to enable certain aspects of realistic flight/performance/etc. Without these, 'realism' is going to be severely limited. Memory footprint (except RSS) is quite small so having them (you likely might already) is not going to produce a performance hit. Therefore, each one of these mods listed as REQUIRED are just that: see next question.

Q: I don't want to install ****** (some required mod), what will happen, what should I do?
A: The mods marked as required are just that--required. Without it, things may break unexpectedly, your game will not play in the manner intended, and your life will be more difficult than it should be. For example, if you don't install FAR, you will need about twice the rocket you would need in real life. Realism Overhaul+RealSolarSystem is designed around these dependencies. We can't offer support if you haven't installed RO and the required mods as directed.

Q: I want to install ********, but it's not on the supported list, what happens?
A: Don't know. If it's not on the list, then it hasn't been tested and therefore there is no guarantee if it will work or not, and there will be NO SUPPORT for any issues that it may cause. If that mod is just parts, be warned now, it's behavior will likely cause you nothing but pain and anguish simply because it's performance is not what it should be, UNLESS it was designed for the real world. Likely it was not and things were 'kerbalized' so as not to be over powered. If you really like that mod, and you want it supported, then put in a request.

Q: My Realism Overhaul installation is broken/doesn't work/etc. What do I do?
A: First, go through these basic FAQ and see if your answer is there. If it is, great, problem solved. If not, follow the prescribed basic troubleshooting steps following the FAQ. If that doesn't fix it, THEN post your issue, following the directions for your post, otherwise be prepared to be chastised.

Q: My engines don't throttle! How can I fix this?
A: You don't, and we won't. It's working as designed. In real life, 99% of rocket engines don't throttle at all. A few throttle down to about 60-70% (like the SSME) to lower crew G forces on launch and to limit stress on the stack (especially at Max Q). Only a tiny, tiny few actually "deeply" throttle--basically just engines designed for landers. The Lunar Module Descent Engine is the largest deeply-throttling engine in RO right now, and will throttle down to about 12%. As more and more engines are added, your options will increase, but again, there aren't many engines that do throttle. Such is life. Welcome to Realism Overhaul.

Q: This mod is unbalanced. Sizes, thrust, mass, Isp, etc just don't make sense. Part A is 2x bigger, 1/10 the thrust and 33% better Isp than part B. This has to be wrong, when will it be fixed?
A: Well...it's not wrong. Seriously. It's not. Remember how I told you you needed unlearn or forget everything you have learned. This is just one of many examples. KSP teaches you bad things about rocket engines. What matters for a rocket engine's thrust is its *throat* diameter, not its nozzle exit diameter. The expansion ratio (exit:throat) varies greatly between different rocket engines, dependent on the altitude at which they are designed to operate. A rocket nozzle is designed to expand the exhaust gas to ambient pressure. At sea level, that's 101.325 kPa. In vacuum...that's 0. Compare the XLR11 with the RL-10, for example. There are dozens/hundreds/thousands of documents out there describing the ins and outs of rocket design. Those are now what you need to know (if you want to learn). "Well, this is how it is/how I did it in [stock] KSP before." isn't the right answer, and almost every time is going to be flat out wrong. Trust us. What you see in Realism Overhaul is how it's supposed to be.

Q: Seriously, this part doesn't have the right numbers compared to what I've read here, here, and here. What should I do?
A: The neat thing with this mod is that if you have a question about performance or something, do the research yourself with the real life component. IF what your research shows is grossly wrong with what you see in Realism Overhaul, then by all means, please point out the error. Typographical errors, or just plain missing something happens, best for everybody involved if you speak up, but PLEASE, do so AFTER you've done your research.

Q: My parts are bigger/smaller than they were?
A: Yep, they sure are, and no, we are not going to change it back. Parts have been resized to approximate or duplicate real life. Such as this is Realism Overhaul, that is kind of the point. It's also because otherwise you may burn up or over-G on reentry; if you go about using real life masses (as Kerbal pods do) with 2/3rds the size, you'll end up with density 4x what it should be, and ballistic coefficient (ratio of surface area to mass) 1/2 what it should be. It's like the difference between reentry with a nuclear warhead, and reentry with a pod designed for crew survival.

Q: Some parts have CoM/CG offset from the true center of the part? AND/OR My craft won't fly straight, what is happening?
A: Some parts (even more will be soon) have had their CoM adjusted to approximate real life. Once again, no, we aren't going to change it back. Soon we will add a moving CoM to further enhance realism. Remember, this Realism Overhaul not Kinda Sorta Maybe Real But Totally Fiction When Things Get Tough Overhaul. Besides, having an offset CoM is very important for making controlled reentries. Your crew might not survive, and your pod might overheat, if you can't fly a lifting reentry.

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

3. GET YOUR LOGS. During your testing if the issue happens make sure you get the log. This is vital. No log, no support. Here's how to get the log we need:

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
Advanced Jet Engine by camlost (v1.4) NON-OFFICIAL 0.24.* update HERE
Deadly Reentry Continued by NathanKell (v5.2)
Engine Ignitor by HoneyFox. (v3.3) [NOTE: DO NOT install the MM files located in the "extract to use" zip files that are included.]
Ferram Aerospace Research by ferram4 (v0.14.1.1)
Module Manager by sarbian, swamp_ig, and ialdabaoth - INCLUDED (v2.2.1)
ModuleRCSFX by ialdabaoth. Updated by NathanKell - INCLUDED
RealChutes by stupid_chris. (v1.2.4)
Real Fuels by NathanKell (v7.2)
Real Solar System by NathanKell (v7.1)

Very Highly Recommended:
Kerbal Joint Reinforcement (v2.4.3)
RemoteTech 2 by Remote Technologies Group (v1.4) [NOTE: If you want to send data back, you need this!]
TAC Life Support by TaranisElsu (v0.9 PRE-RELEASE #4)
TweakScale by Biotronic (v1.37)

Recommended:
CrossFeedEnabler by NathanKell (v2.2)
MechJeb by many (v2.3.1)
ModuleFixer by arsenic87. Download HERE.
PartCatalog by BlackNecro (v3.0RC5)
TextureReplacer by Shaw (v1.6.1)
or Active Texture Management by rbray89 (v3.3.1)
Toolbar by blizzy78 (v1.7.6)

Supported Mods/Part Packs:
6S Service Compartment Tubes by Nothke
AIES by carmics (v1.5.1)
ALCOR by alexustes
Fustek Station Parts [NOTE: For X0.04-4 DEV BUILD - 5 June 2014]
KW Rocketry by Winston & Kickasskyle (v2.6c) [NOTE: This mod removes shrouds. DO NOT install the "No Shroud" OR "Instant Power" configs.]
PorkWorks Inflatable Habs by Porkjet (v0.4)
Procedural Dynamics - Procedural Wing by DYJ (v0.8)
Procedural Fairings by e-dog (v3.09)
Procedural Parts by swamp_ig (v0.9.18)
RLA Download HERE.
Rocketdyne F-1 by 1096bimu. Download HERE.
Soviet Engines by BobCat
Space Shuttle Engines & KM_Gimbal by dtobi (v2.0) [NOTE: Limited to detailed SSMEs, 1x OMS, Tanks]
Universal Storage by PaulKingtiger (v0.75.5) [NOTE: Core and KAS packages only]

Optional:
Realistic Tech Progression Lite tech tree by Medieval Nerd (note: requires RftSEngines)

Coming Soon:
Works in Progress or Future Plans - No Particular Order [NOTE: Packs in this list ARE NOT guaranteed to work at all, and could quite possibly cause KSP to crash]
Near Future Technologies by Nertea (v2.*) (WIP)
NovaPunch by Tiberion (v2.05) (WIP)
Spaceplane Plus by Porkjet
B9 by bac9 (WIP)
FASA by Frizzank (v4.97) (WIP)
World Space by Lovad (WIP)
KerbX by Borklund (WIP)
LazTek SpaceX by LazurusLuan (WIP)
Taurus HCV by jnrobinson (v1.2.1) (WIP)
Aerojet Kerbodyne (WIP)
OLDD Apollo/Saturn (WIP)
Sum Dum Heavy Industries - Service Module System by sumghai (v2.0)
KOSMOS Spacecraft Design Bureau by Normak (4.7) (WIP)
SCANsat by technogeeky
Lionhead ESA, Rovers, Landers, Probes, Solar Panels by Yogui87
Kethane by Majiir
SkyLab by raidernick
International Space Station Group Project
Component Space Shuttle maintained by Dragon01
Chaka Monkey Exploration Systems by YANFRET
USI Kolonization Systems (MKS/OKS) by RoverDude
Kerbal Attachment System by Majiir
B.Dynamics by BahamutoD

Changelog:

v6.0 ALPHA 10 \/
*Small fixes to RLA, KW
*AIES Complete
*SRB Thrust Curves - WIP - A LOT Better
*Some minor B9 work - still a long way to go
*Some FASA work and bug fixes
*TACLS and PP changes
*Klockheed Martian update complete
*Porkworks update complete
*Nothke Service Module update complete
*FusTek update complete
*NearFuture Construction update complete
*Merlin 1D Vac bug fix
*NearFuture Propulsion - WIP

v6.0 ALPHA 9 \/
*EngineIgnitor Fixes
*Some more work on KW SRBs, ADDED GEM40, Isp fixes
*Typographical Error Fixes
*LazTek parachute fix
*RLA PowerGeneration, ElectricEngines, Stockalike now fully supported!
*Updated TweakScale stuff with latest release
*Updated KW Fuel Tanks for RealFuels update
*Updated RealChutes to work wih MM files now included.
*Start AIES rework (very much a WIP)
*Minor KW Engine fixes

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