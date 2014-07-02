#if UNITY_EDITOR
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

/**
 *	\brief 
 */
public class QuickStart : EditorWindow
{
	[MenuItem("Window/ProBuilder/Run Update or Install", false, 1)]
	public static void InstallUpdate()
	{
		int packNo = GetProBuilderPackVersion();
		if(packNo < 0)
		{
			if(EditorUtility.DisplayDialog("Install ProBuilder Update", "No upgrade package found!", "Okay", ""))
				return;
			else
				return;
		}

		Selection.activeTransform = null;

		if(EditorUtility.DisplayDialog("Install ProBuilder Update", "Install ProBuilder r" + packNo + "\n\nWarning!  Back up your project!",
			"Run Update or Install", "Cancel"))
			LoadLatestPack();
	}

	public static void LoadLatestPack()
	{
		// remove old GUI path
		string oldGUIPath = "Assets/6by7/Shared/GUI";
		
		DeleteDirectory(oldGUIPath);
			
		string pb_path = GetProBuilderPack();
	
		if(pb_path == "") return;
		
		AssetDatabase.ImportPackage(pb_path, false);
	}

	public static string GetProBuilderPack()
	{
		string[] allFiles = Directory.GetFiles("Assets/6by7/ProBuilder/Install/", "*.*", SearchOption.AllDirectories);
		string[] allPackages = System.Array.FindAll(allFiles, name => name.EndsWith(".unitypackage"));
		string pack = "";
		// if(allPackages.Length > 3)	
		// 	Debug.LogWarning("Whoa, your project is full of unitypackages.");

		// sort out non ProBuilder2v- packages
		int highestVersion = 0;
		for(int i = 0; i < allPackages.Length; i++) {
			if(allPackages[i].Contains("ProBuilder2-v")) {

				// probably not necessary here, but regular expressions are super cool.  
				string pattern = @"[v0-9]{4,6}";
				MatchCollection matches = Regex.Matches(allPackages[i], pattern, RegexOptions.IgnorePatternWhitespace);

				int revision = -1;
				foreach(Match m in matches)
					revision = int.Parse(m.ToString().Replace("v", ""));
				
				if(revision < 1)
					continue;

				if(revision > highestVersion) {
					highestVersion = revision;
					pack = allPackages[i];
				}
			}
		}

		#if !UNITY_STANDALONE_OSX && !UNITY_IPHONE
		pack = pack.Replace("\\", "/");
		#endif
		// Debug.Log("Importing ProBuilder revision number " + highestVersion);
		return pack;
	}

	public static bool ProBuilderExists()
	{
		string[] allFiles = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
		string[] allLibs = System.Array.FindAll(allFiles, name => name.EndsWith(".dll"));

		for(int i = 0; i < allLibs.Length; i++)
			if(allLibs[i].Contains("ProBuilder"))
				return true;
		return false;
	}

	public static void DeleteDirectory(string path)
	{
		if(!Directory.Exists(path))
			return;

		string[] files = Directory.GetFiles(path);
		string[] dirs = Directory.GetDirectories(path);

		foreach (string file in files)
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}

		foreach (string dir in dirs)
		{
			DeleteDirectory(dir);
		}

		Directory.Delete(path, false);

		if(File.Exists(path+".meta"))
			File.Delete(path+".meta");
    }

	public static int GetProBuilderPackVersion()
	{
		string[] allFiles = Directory.GetFiles("Assets/6by7/ProBuilder/Install/", "*.*", SearchOption.AllDirectories);
		string[] allPackages = System.Array.FindAll(allFiles, name => name.EndsWith(".unitypackage"));

		// if(allPackages.Length > 3)	
		// 	Debug.LogWarning("Whoa, your project is full of unitypackages.");

		// sort out non ProBuilder2v- packages
		int highestVersion = -1;
		for(int i = 0; i < allPackages.Length; i++) {
			if(allPackages[i].Contains("ProBuilder2-v")) {

				// probably not necessary here, but regular expressions are super cool.  
				string pattern = @"[v0-9]{4,6}";
				MatchCollection matches = Regex.Matches(allPackages[i], pattern, RegexOptions.IgnorePatternWhitespace);

				int revision = -1;
				foreach(Match m in matches)
					revision = int.Parse(m.ToString().Replace("v", ""));
				
				if(revision < 1)
					continue;

				if(revision > highestVersion) {
					highestVersion = revision;
				}
			}
		}
		// Debug.Log("Importing ProBuilder revision number " + highestVersion);
		return highestVersion;
	}
}

class MyAllPostprocessor : AssetPostprocessor 
{	
	static void OnPostprocessAllAssets (
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach(var str in importedAssets)
		{
			if(str.Contains("ProBuilderCore.dll"))
			{
				System.Type upgradeUtil = System.Type.GetType("pb_Upgrade_Utility");
				MethodInfo onUpgrade = upgradeUtil.GetMethod("OnProBuilderUpgrade");
				onUpgrade.Invoke(null, null);
				break;
			}
		}
	}

	public static string[] GetScenes()
	{
		string[] allFiles = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
		string[] allScenes = System.Array.FindAll(allFiles, name => name.EndsWith(".unity"));
		return allScenes;
	}
}

#endif