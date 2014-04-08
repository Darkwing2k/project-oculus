using UnityEngine;
using System.Collections;

public interface ICVar {
	object Value { get; set; }
	object DefaultValue { get; }
	string Name { get; }
	CVarFlag Flags { get; }
	string Description { get; }

	void Reset();
}
