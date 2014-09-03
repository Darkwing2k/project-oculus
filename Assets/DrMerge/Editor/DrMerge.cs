using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class DrMerge : EditorWindow 
{

    private SceneManager manager;
    private Object sceneA;
    private Object sceneB;

    private bool parserDone;

    [MenuItem("Window/DrMerge")]
    static void Init()
    {
        DrMerge window = (DrMerge)EditorWindow.GetWindow<DrMerge>("DrMerge");
        window.Show();

        window.position = new Rect(800, 100, 600, 800);
    }

    private void reset(bool delete)
    {
        if (delete)
        {
            AssetDatabase.Refresh();

            EditorApplication.OpenScene(SceneManager.sceneFolderPath + "/" + manager.sceneA.name + ".unity");
            AssetDatabase.DeleteAsset(SceneManager.mergedScenePath);
        }

        manager = null;
        parserDone = false;
        Change.changeCount = 0;
    }

    private void OnLostFocus()
    {
        //reset();
    }

    private void OnDestroy()
    {
        Debug.Log("DrMerge destroyed");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reset"))
        {
            reset(true);
        }

        if (!parserDone)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Scene A");
            sceneA = EditorGUILayout.ObjectField(sceneA, typeof(Object), true);

            GUILayout.Label("Scene B");
            sceneB = EditorGUILayout.ObjectField(sceneB, typeof(Object), true);

            if (GUILayout.Button("Start"))
            {
                int lastIndexOfSlash = AssetDatabase.GetAssetPath(sceneA).LastIndexOf('/');
                string sceneFolderPath = AssetDatabase.GetAssetPath(sceneA).Substring(0, lastIndexOfSlash);

                manager = new SceneManager(AssetDatabase.GetAssetPath(sceneA), AssetDatabase.GetAssetPath(sceneB), sceneFolderPath);
                manager.parse();

                manager.compare();

                manager.print(true);

                parserDone = true;
            }

            EditorGUILayout.EndVertical();
        }

        if (parserDone)
        {
            manager.onGUI(position);

            if (manager.jobsDone)
            {
                if (GUILayout.Button("Done"))
                {
                    reset(false);
                    AssetDatabase.Refresh();

                    GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject>();

                    foreach (GameObject currentGO in allGOs)
                    {
                        currentGO.name = currentGO.name.Split(SceneManager.SEPSYM)[0];
                    }

                    EditorApplication.SaveScene();
                }
            }
        }
    }
}
