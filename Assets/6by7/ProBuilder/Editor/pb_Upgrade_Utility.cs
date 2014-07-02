using UnityEngine;
using UnityEditor;
using System.Collections;

/**
 *	\brief Write any post-install code here.
 *	OnProBuilderUpgrade will be called whenever a ProBuilder upgrade script is run.
 */
public class pb_Upgrade_Utility : Editor
{
	public static void OnProBuilderUpgrade()
	{
		pb_Editor_Utility.ForceRefresh(false);	// meshes need to be refreshed for some reason that I cannot recall.
		ProBuilderMenuItems.ForceCloseEditor();	// force close editor, which allows the new GUI to load resources.
		UpgradeSceneCollisions();				// r961+ uses Unity's component system for colliders
	}

	public static void UpgradeSceneCollisions()
	{
		pb_Entity[] objs = (pb_Entity[])GameObject.FindObjectsOfType(typeof(pb_Entity));
		foreach(pb_Entity ent in objs)
			ent.GenerateCollisions();
	}

	// for most users this should not be necessary
	// [MenuItem("Window/ProBuilder/Actions/Refresh Project Collisions")] 
	public static void UpgradeSceneCollisionsProjectWide()
	{
		string[] allFiles = System.IO.Directory.GetFiles("Assets/", "*.*", System.IO.SearchOption.AllDirectories);
		string[] allScenes = System.Array.FindAll(allFiles, name => name.EndsWith(".unity"));
		string currentScene = EditorApplication.currentScene;
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		int objCount = 0;
		
		foreach(string cheese in allScenes)
		{
			EditorApplication.OpenScene(cheese);
			pb_Entity[] objs = (pb_Entity[])GameObject.FindObjectsOfType(typeof(pb_Entity));
			foreach(pb_Entity ent in objs)
				ent.GenerateCollisions();
			objCount += objs.Length;
			EditorApplication.SaveScene(cheese, false);
		}

		EditorApplication.OpenScene(currentScene);

		EditorUtility.DisplayDialog("Update Collisions", "Collisions successfully refreshed!\n\nRefreshed " + objCount + " ProBuilder objects across " + allScenes.Length + " scenes.", "Okay");

	}	
}
