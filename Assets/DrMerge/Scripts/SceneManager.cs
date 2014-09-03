using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneManager
{
    public DeserializedScene sceneA;
    public DeserializedScene sceneB;

    public static string sceneFolderPath;
    public static string mergedScenePath;
    public static string logPath;
    public const char SEPSYM = '¯';
    public Color prevColor;

    public bool jobsDone;

    public SceneManager(string pathToA, string pathToB, string sceneFolderPath)
    {
        List<string> allLines = new List<string>();

        using(StreamReader fin = new StreamReader(pathToA))
        {
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                allLines.Add(line);
            }
        }

        sceneA = new DeserializedScene(this, allLines);

        string sceneNameA = pathToA.Substring(pathToA.LastIndexOf('/') + 1);
        sceneA.name = sceneNameA.Split('.')[0];

        allLines = new List<string>();

        using (StreamReader fin = new StreamReader(pathToB))
        {
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                allLines.Add(line);
            }
        }

        sceneB = new DeserializedScene(this, allLines);

        string sceneNameB = pathToB.Substring(pathToB.LastIndexOf('/') + 1);
        sceneB.name = sceneNameB.Split('.')[0];

        sceneA.opponent = sceneB;
        sceneB.opponent = sceneA;

        SceneManager.sceneFolderPath = sceneFolderPath;
        SceneManager.mergedScenePath = sceneFolderPath + "/Merged_" + sceneA.name + "_" + sceneB.name + ".unity";
        SceneManager.logPath = Application.dataPath + "/DrMerge/Logs";
    }

    public void compare()
    {
        DeserializedScene.compare(sceneA, sceneB);

        printMerged(false);

        EditorApplication.OpenScene(mergedScenePath);
    }

    public void parse()
    {
        sceneA.parse();
        sceneB.parse();
    }

    public void print(bool debug)
    {
        string targetPathA = "";
        string targetPathB = "";

        if (debug)
        {
            targetPathA = logPath + "/ParserLogA.txt";
            targetPathB = logPath + "/ParserLogB.txt";
        }
        else
        {
            targetPathA = sceneFolderPath + "/" + sceneA.name + "_tmp.unity";
            targetPathB = sceneFolderPath + "/" + sceneB.name + "_tmp.unity";
        }

        using (StreamWriter fout = new StreamWriter(targetPathA, false))
        {
            sceneA.print(fout, debug);
        }

        using (StreamWriter fout = new StreamWriter(targetPathB, false))
        {
            sceneB.print(fout, debug);
        }
    }

    public void printMerged(bool debug)
    {
        string targetPath = "";

        if (debug)
        {
            targetPath = logPath + "/MergedScene.txt";
        }
        else
        {
            targetPath = mergedScenePath;
        }

        using (StreamWriter fout = new StreamWriter(targetPath, false))
        {
            sceneA.print(fout, true, true, debug);
            sceneB.print(fout, true, false, debug);
        }
    }

    public void onGUI(Rect editorWindow)
    {
        List<Change> deleteAfter = new List<Change>();

        foreach (Change currentChange in sceneA.changes)
        {
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));

            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));

            currentChange.expanded = EditorGUILayout.Foldout(currentChange.expanded, "Change " + currentChange.nr);

            if (GUILayout.Button("<"))
            {
                currentChange.setChosen(true);

                printMerged(false);

                openSceneKeepSelection(mergedScenePath);
            }

            if (GUILayout.Button(">"))
            {
                currentChange.setChosen(false);

                printMerged(false);

                openSceneKeepSelection(mergedScenePath);
            }

            if (GUILayout.Button("Save"))
            {
                deleteAfter.Add(currentChange);
            }

            GUILayout.EndHorizontal();

            if (currentChange.expanded)
            {
                List<long> allIDs = getAllChangeIDs(currentChange);                

                foreach (long currentID in allIDs)
                {
                    bool isGO = false;

                    changeColor(new Color(0, 0, 0));
                    GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
                    resetColorToPrevious();

                    isGO = sceneA.drawChangedObjects(currentID, currentChange, editorWindow);
                    isGO = sceneB.drawChangedObjects(currentID, currentChange.partner, editorWindow);

                    GUILayout.EndHorizontal();

                    if (isGO)
                    {
                        showSelectButton(currentID);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        foreach (Change toDelete in deleteAfter)
        {
            sceneA.changes.Remove(toDelete);
            sceneB.changes.Remove(toDelete.partner);
        }

        if (sceneA.changes.Count == 0 && sceneB.changes.Count == 0 && !jobsDone)
        {
            jobsDone = true;
        }

        deleteAfter.Clear();
    }

    private void openSceneKeepSelection(string path)
    {
        string selectedName = "";
        bool somethingWasSelected = false;

        if (Selection.activeGameObject != null)
        {
            selectedName = Selection.activeGameObject.name;
            somethingWasSelected = true;
        }

        EditorApplication.OpenScene(path);

        if (somethingWasSelected)
        {
            Selection.activeGameObject = GameObject.Find(selectedName);
        }
    }

    private List<long> getAllChangeIDs(Change currentChange)
    {
        List<long> allIDs = new List<long>();

        foreach (long ID in currentChange.references)
        {
            if (!allIDs.Contains(ID))
                allIDs.Add(ID);
        }
        foreach (long ID in currentChange.partner.references)
        {
            if (!allIDs.Contains(ID))
                allIDs.Add(ID);
        }

        return allIDs;
    }

    private void showSelectButton(long currentID)
    {
        if (GUILayout.Button("Select GO"))
        {
            GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject currentGO in allGOs)
            {
                if (currentGO.name.EndsWith("" + currentID))
                {
                    Selection.activeGameObject = currentGO;
                }
            }
        }
    }

    public void changeColorOnCondition(bool condition, Color trueCase, Color falseCase)
    {
        if (condition)
        {
            changeColor(trueCase);
        }
        else
        {
            changeColor(falseCase);
        }
    }

    public void changeColor(Color desiredColor)
    {
        prevColor = GUI.color;
        GUI.color = desiredColor;
    }

    public void resetColorToPrevious()
    {
        GUI.color = prevColor;
    }
}


