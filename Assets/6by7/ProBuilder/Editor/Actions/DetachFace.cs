using UnityEngine;
using UnityEditor;
using System.Collections;

public class DetachFace : Editor
{
	[MenuItem("Window/ProBuilder/Actions/Detach Face(s)")]
	public static void Detach()
	{
		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
			foreach(pb_Face face in pb.selected_faces)
				pb.DetachFace(face);
	}
}
