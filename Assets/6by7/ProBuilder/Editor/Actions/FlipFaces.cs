using UnityEngine;
using UnityEditor;
using System.Collections;

public class FlipFaces : Editor {

	[MenuItem("Window/ProBuilder/Actions/Flip Object Normals")]
	public static void FlipObjectNormals()
	{
		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
			pb.ReverseWindingOrder();
	}


	[MenuItem("Window/ProBuilder/Actions/Flip Face Normals")]
	public static void FlipFaceNormals()
	{
		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
		{
			pb.ReverseWindingOrder(pb.selected_faces.ToArray());
		}
	}	
}
