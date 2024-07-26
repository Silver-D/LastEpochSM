# Last Epoch SM
## **Last Epoch Simple Modules**
## What Is This
This is a Module-based Mod for the **Last Epoch** game using the [Melon Loader](https://github.com/LavaGang/MelonLoader) framework.
It comes with the main Module that is the _LastEpochSM.dll_ that by itself does nothing more than load other modules.
Currently, there are 2 modules that are loaded by this loader - <br />
The **LastEpochSM_Mini** and the **LastEpochSM_Skins**. You can install one or ther other, or both, but the main _LastEpochSM.dll_
loader is required for both of them to work.

The goal of this is to have a simple, off-hands Module loader that does all the dirty setup for other mods to simply use without setting up all the Game Event Listeners,
Asset Loaders, Conf Loaders, and etc.<br />Here is what each of these modules do:

## LastEpochSM_Mini
Made to be compatible with [LastEpoch_Hud](https://github.com/RCInet/LastEpoch_Mods), it contains some small options that the Hud lacks, or implements in a slightly different way.<br />
This Module has no visual options - everything is configured via its .conf file.<br />Please read the included _ReadMe_ to read a detailed explanation of how these settings work.
Here is a brief summary of the settings so far:
### Forge
+ __noForgingPotentialCost__ : Disables the in-game Forging Potential cost when crafting items.
+ __alwaysSealAffixWithGlyph__ : Always seal a desired affix when upgrading an affix with a Glyph of Despair.
+ __minImplicitRangePercentInt, minAffixRangePercentInt__ : Sets a minimum desired threshold for values when crafting/rerolling items.<br />
While _LastEpoch_Hud_ has options to apply affix values to an item when it
is inserted into the Crafting Slot, the options in this module allow for a little bit
of controlled randomness during the actual forging process-- rather than setting a number
of specific, deterministic values.<br />
It also kind of applies the configured _minAffixRangePercentInt_ value to rolls on Unqiue Affixes on Unqie Items, if you use the LastEpoch_Hud's crafting system,
but this needs to be further refined and tested when that feature is fully complete.<br />
Note that the values are integers, 50 is 50%.

### Monolith
+ __noLoseWhenDie__ : Don't lose stuff when you die in a Monolith.
While _LastEpoch_Hud_ has a similar checkbox, it actually does nothing at
this time. This setting has been tested with Monoliths at Normal and Empowered
Timelines (no stability was decreased), but I have yet to test it while having some
Gaze of Orobyss. It should work, though. I hope. I need to play more instead of
working on this.
+ ___enableBlessingTransfers___ : Blessing Transfers are enabled, and the _BlessingTransfers.json_ file is processed
+ ___Blessing Transfers___ : As this Mod is called Simple Modules, it is only fitting that this configuration
file may induce some headaches to configure.<br />
+ But essentially, when properly configured, this section allows Transfers of
Blessings from one Monolith Timeline into another.<br />
This makes it possible to have, for example, _Critical Strike Multiplier_ and _Chritical Hit Chance_ blessings on
different Timelines --<br />
allowing you to finally have the _option of equipping both
of these blessings at the same time_.
+ It also allows you to remove useless blessings from Timelines, making it much
easier to farm for specific blessings -- and the freedom to fully configure an
entire set of Timeline Blessings in order to have a desired combination of blessings
equipped at all times.
+ This works for both the Blessing Rewards that you get when beating the Timeline Boss,
and even works properly for blessings that you have already unlocked and discovered.<br />
And if you have those blessings in question equipped, you just need to unequip them
for them to jump to their configured Timelines.
+ You need to have more than one blessing discovered in a timeline in order to equip a
different blessing -- just something to keep in mind when moving blessings around, as the blessings don't switch Timeline slots while they are equipped.
+ Either way, a full (and an embarassing) explanation of the format of this is in the
[_Blessing Transfers Format Explained.txt_](https://github.com/Silver-D/LastEpochSM/blob/master/Mods/Mini/ReadMe/2.%20Blessing%20Transfers%20Format%20Explained.txt) document.
+ An example config file is also included with the Release. Have fun.
## LastEpochSM_Skins
This is essentially a port of a module that was a part of _LastEpoch_Mods 3.2.3_.<br />
It unlocks the "Appearance" tab of your character, and allows you to set an appearance of an equippable item to another.<br />
No any DLC online cosmetics are available, nor can they be.<br />
The [LastEpoch_Hud](https://github.com/RCInet/LastEpoch_Mods) has been missing this feature since version 4.0, so I decided to clean it up a bit and port it to its own module,<br />
supported by the _LastEpochSM.dll_ framework.<br />
As such, all of the internal code belongs to Ash, the Author of [LastEpoch_Hud](https://github.com/RCInet/LastEpoch_Mods).<br />

Stuff has been cleaned up, and some some stuff has been moved around and made easier to maintain -- especially when it uses a lot of the features that the _LastEpochSM.dll_ provides. Some changes have been made on the user-level as well, and they are:
+ Added Catalysts as an appearance option to the Offhand Selection.
+ Fixed an issue with the mod "forgetting" where the appearance config file is stored for the current character when switching between characters created in different Cycles.
+ Changed the logic of how the per-character configuration determines where the character-specific config should be stored.<br />This makes it a bit more future-proof -- however, the directories where the configuration files are saved now use a different schema.
+ There was an attempt to fix the "stretching" of some Slot Icon graphics, but the attempt was very valiant. It's better, but I do not understand UI code. It's a bit better, though.
+ As this is a port of the _LastEpoch_Mods 3.2.3_ code, the issue where some appearance options weren't being loaded after loading a character has inadvertently been fixed.
+ This issue is present in the _LastEpoch Skins Standalone_, that is another standalone version that's found on the _Nexus_.
+ Please __do not use any of the above listed Skin mods when using this version of the _Skins mod___.<br />Use the latest and greatest [LastEpoch_Hud](https://github.com/RCInet/LastEpoch_Mods), and only ___one___ of the Skin mods - this, or the Standalone version from the Nexus.<br />
+ _Do not have both installed_.
## Compatibility :
+ LastEpochSM is made to be compatible with the [LastEpoch_Hud](https://github.com/RCInet/LastEpoch_Mods), but **LastEpochSM_Skins** _is incompatibe_ with:
+ The **LastEpoch_Mods Skins Standalone** that's found on the _Nexus_ is _incompatible_.
+ If you wish to use this version of the Skins mod, please disable that Mod in your installation directory by moving it to a different folder.
+ While it was thoroughly tested with _LastEpoch_Hud_, these are different projects by different people, and as such, some compatibility issues may arise, especially since both of these projects use very similar methods to patch the game's code.<br />
Just a forewarning if something, at some point, breaks between the two.
## How to :
+ Install [Melon Loader 0.5.7](https://github.com/LavaGang/MelonLoader) ([0.5.7 specifically](https://github.com/LavaGang/MelonLoader/releases/tag/v0.5.7))
+ Launch your game once.
+ Download the latest [Release](https://github.com/Silver-D/LastEpochSM/releases) or compile yourself,
and put the files into the game's installation directory, in _"Last Epoch\Mods\"_.
+ If you don't want to use skins, you don't have to install the _LastEpochSM_Skins.dll_ module,<br />
or if you just want skins, you can skip the _LastEpochSM_Mini.dll_ module.
+ _LastEpochSM.dll_ needs to be installed for both of these modules to work.
+ If you're using the _LastEpochSM_Mini.dll_ module, configure your desired settings in _Mini\Conf.json_, as everything is turned off by default.
+ The ReadMe files are included with details about the [setting options](https://github.com/Silver-D/LastEpochSM/blob/master/Mods/Mini/ReadMe/1.%20Config%20Settings.txt), and an explanation on how to setup the [_Blessing Transfers_](https://github.com/Silver-D/LastEpochSM/blob/master/Mods/Mini/ReadMe/2.%20Blessing%20Transfers%20Format%20Explained.txt) feature.
+ Launch your game. Look at the MelonLoader Console for any issues.
## Requirements :
+ [Melon Loader 0.5.7](https://github.com/LavaGang/MelonLoader/releases/tag/v0.5.7) (Only use this version)
