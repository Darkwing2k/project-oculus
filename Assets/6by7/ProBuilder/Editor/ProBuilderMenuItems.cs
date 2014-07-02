using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProBuilderMenuItems : EditorWindow
{
	const int SECTION = 15;

#region WINDOW

	[MenuItem("Window/ProBuilder/ProBuilder Window", false, SECTION + 0)]
	public static pb_Editor OpenEditorWindow()
	{
		if(EditorPrefs.HasKey("defaultOpenInDockableWindow") && !EditorPrefs.GetBool("defaultOpenInDockableWindow"))
			return (pb_Editor)EditorWindow.GetWindow(typeof(pb_Editor), true, "ProBuilder", true);			// open as floating window
		else
			return (pb_Editor)EditorWindow.GetWindow(typeof(pb_Editor), false, "ProBuilder", true);			// open as dockable window
	}

	[MenuItem("Window/ProBuilder/Texture Window", false, SECTION + 1)]
	public static void OpenTextureWindow()
	{
		pb_Editor.editorInstance.SetEditLevel(pb_Editor.EditLevel.Texture);
	}

	[MenuItem("Window/ProBuilder/Open Shape Menu %#k", false, SECTION + 2)]
	public static void ShapeMenu()
	{
		EditorWindow.GetWindow(typeof(pb_Geometry_Interface), true, "Shape Menu", true);
	}

	public static void ForceCloseEditor()
	{
		EditorWindow.GetWindow<pb_Editor>().Close();
	}
#endregion

#region ProBuilder/Edit

	[MenuItem("Window/ProBuilder/Edit/Toggle Edit Level")]
	public static void ToggleEditLevel()
	{
		pb_Editor.editorInstance.ToggleEditLevel();
	}

	[MenuItem("Window/ProBuilder/Edit/Toggle Selection Mode")]
	public static void ToggleSelectMode()
	{
		pb_Editor.editorInstance.ToggleSelectionMode();
	}
#endregion	
}