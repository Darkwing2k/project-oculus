using UnityEngine;
using UnityEditor;
using System.Collections;

public class ForceMeshRefresh : Editor {

	[MenuItem("Window/ProBuilder/Actions/Force Refresh Objects")]
	public static void Inuit()
	{
		pb_Editor_Utility.ForceRefresh(true);
	}
}
