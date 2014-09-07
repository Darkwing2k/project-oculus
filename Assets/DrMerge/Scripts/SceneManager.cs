using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneManager
{
    public ChangeManager changeManager;

    public DeserializedScene sceneA;
    public DeserializedScene sceneB;

    public static string sceneFolderPath;
    public static string mergedScenePath;
    public static string logPath;
    public const char SEPSYM = '¯';

    public Vector2 scrollPos;

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

        sceneA.fillRefSets();
        sceneB.fillRefSets();

        HashSet<long> changeIDs = new HashSet<long>();

        DeserializedScene.changeCount = 0;

        sceneA.getAllIDsOfChangedComponents(changeIDs);
        sceneB.getAllIDsOfChangedComponents(changeIDs);

        changeManager = new ChangeManager(this, changeIDs);

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

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (ChangeView currentView in changeManager.changeViews.Values)
        {
            currentView.onGUI(editorWindow, deleteAfter);
        }

        GUILayout.EndScrollView();

        foreach (Change toDelete in deleteAfter)
        {
            changeManager.removeChange(toDelete);
        }

        if (changeManager.changes.Count == 0 && !jobsDone)
        {
            jobsDone = true;
        }

        deleteAfter.Clear();
    }
}


