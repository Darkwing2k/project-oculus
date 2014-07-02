using UnityEngine;
using UnityEditor;
using System.Collections;

public class StripProBuilderScripts : Editor 
{
	[MenuItem("Window/ProBuilder/Actions/Strip ProBuilder Scripts")]
	public static void StripAllScenes()
	{
		pb_Object[] all = (pb_Object[])FindObjectsOfType(typeof(pb_Object));

		for(int i = 0; i < all.Length; i++)
		{
			if(all[i].GetComponent<pb_Entity>())
				DestroyImmediate(all[i].GetComponent<pb_Entity>());
			DestroyImmediate(all[i]);
		}
	}
}
