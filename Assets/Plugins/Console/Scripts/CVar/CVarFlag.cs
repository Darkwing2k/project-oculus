using UnityEngine;
using System;

[Flags]
public enum CVarFlag {
	ALL = ~0,

	ROM = 1 << 0, // The end user can not change this CVar
	INIT = 1 << 1, // Must be set via commandline arguments during startup
	CHEAT = 1 << 2, // Cheats must be enabled to change this CVar
	DEVELOPER = 1 << 3, // Must be in developer mode to see and change this CVar
	SYSTEM = 1 << 4 // A System CVar
}
