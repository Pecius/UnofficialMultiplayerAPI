# How to use the API

Download and copy 0UnofficialMultiplayerAPI.dll to your mod's _Assemblies_ directory and reference it in your project.

Check the API documentation and examples for further details.

# 0UnofficialMultiplayerAPI

This is basically an interface, a glorified header file in an assembly with some additional sugar coating.

It acts as a movable bridge between your mod and the API host, it only depends on the game itself so your mod won't suddenly become dependent on the multiplayer mod.

Since only one assembly with the same name (and assembly version) is loaded at the time, your mod will always use its newest version (that is included with the host), even if you referenced an older one.

Thankfully that doesn't mean your mod will break if that happens, as additions of the new classes and methods are not changing much from the point of view of your assembly.

Additionally it also informs clients that they're missing the required host addon and points them either to workshop or to releases on github (if your mod uses the API and multiplayer mod is detected as well).

# How to compile the API

Place the repo root in a _Unofficial Multiplayer API_ folder inside of _Mods_ directory (_RimWorld/Mods_).

You also need the [Multiplayer mod](https://github.com/Zetrith/Multiplayer) placed in the same directory.

To compile it simply use the solution called _Unofficial_multiplayer_API.sln_ found in _Sources_ directory.