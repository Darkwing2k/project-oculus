using System;

[Flags]
public enum CCmdFlags  {
	NONE = 0,
	ALL = ~0, // = -1

	SYSTEM = 1 << 0,
	DEVELOPER = 1 << 1,
	CHEAT = 1 << 2
}
