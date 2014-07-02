using UnityEngine;
using UnityEditor;
using System.Collections;

public class VertexColorInterface : EditorWindow
{
#region CONSTANTS

	public static Color[] COLOR_ARRAY = new Color[10]
	{
		Color.white,
		Color.red,
		Color.blue,
		Color.yellow,
		Color.green,
		Color.cyan,
		Color.black,
		Color.magenta,
		Color.gray,
		new Color(.4f, 0f, 1f, 1f)
	};
#endregion

#region MENU SHORTCUTS

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 1 &1")]
	public static void SetPreset1()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[0]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 2 &2")]
	public static void SetPreset2()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[1]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 3 &3")]
	public static void SetPreset3()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[2]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 4 &4")]
	public static void SetPreset4()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[3]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 5 &5")]
	public static void SetPreset5()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[4]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 6 &6")]
	public static void SetPreset6()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[5]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 7 &7")]
	public static void SetPreset7()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[6]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 8 &8")]
	public static void SetPreset8()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[7]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 9 &9")]
	public static void SetPreset9()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[8]);
	}

	[MenuItem("Window/ProBuilder/Vertex Colors/Set Selected Faces to Preset 0 &0")]
	public static void SetPreset0()
	{
		VertexColorInterface.SetFaceColors(VertexColorInterface.COLOR_ARRAY[9]);
	}
#endregion

#region INITIALIZATION

	[MenuItem("Window/ProBuilder/Vertex Colors/Vertex Colors Window")]
	public static void Init()
	{
		EditorWindow.GetWindow<VertexColorInterface>(true, "Vertex Colors", true);
	}
#endregion

#region ONGUI

	// Color32 col = Color.white;
	public void OnGUI()
	{
		this.minSize = new Vector2(404, 68);
		this.maxSize = new Vector2(404, 68);

		// Debug.Log(Screen.width + ", " + Screen.height);

		GUILayout.BeginHorizontal();

		for(int i = 0; i < COLOR_ARRAY.Length; i++)
		{
			GUI.backgroundColor = COLOR_ARRAY[i];

			GUILayout.BeginVertical();

			if(GUILayout.Button("", 
				GUILayout.MinWidth(36), GUILayout.MaxWidth(36),
				GUILayout.MinHeight(36), GUILayout.MaxHeight(36)))
				SetFaceColors(COLOR_ARRAY[i]);

			COLOR_ARRAY[i] = EditorGUILayout.ColorField(COLOR_ARRAY[i], GUILayout.MinWidth(36), GUILayout.MaxWidth(36));

			GUILayout.EndVertical();

		}

		GUI.backgroundColor = Color.white;

		GUILayout.EndHorizontal();
		
		// col = EditorGUILayout.ColorField(col);
	
		// if(GUILayout.Button("Set Face Color"))
		// 	SetFaceColors(col);
	}
#endregion

#region FUNCTION

	public static void SetFaceColors(Color32 col)
	{
		pb_Editor_Utility.ShowNotification("Set Face Color\n" + col, "");

		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
		{
			foreach(pb_Face face in pb.selected_faces)
				pb.SetFaceColor(face, col);
		}
	}

	public void ShowAllVertexColors()
	{
		pb_Object pb = Selection.activeTransform.GetComponent<pb_Object>();
		
		if(pb == null)
			return;

		Color32[] vColors = pb.msh.colors32;

		for(int i = 0; i < vColors.Length; i++)
			vColors[i] = EditorGUILayout.ColorField(vColors[i]);

		pb.SetColors32(vColors);
	}
#endregion
}
