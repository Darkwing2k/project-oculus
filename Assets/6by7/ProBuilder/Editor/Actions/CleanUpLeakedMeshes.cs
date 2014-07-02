using UnityEngine;
using System.Collections;
using UnityEditor;

public class CleanUpLeakedMeshes : Editor
{

	[MenuItem("Window/ProBuilder/Actions/Clean Up Leaked Meshes")]
	public static void CleanUp()
	{
		if(EditorUtility.DisplayDialog("Clean Up Leaked Meshes?",
			"Cleaning leaked meshes will permenantly delete any deleted pb_Objects, are you sure you don't want to undo?", "Clean Up", "Stay Dirty"))
		{
			EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
		}
	}
}
