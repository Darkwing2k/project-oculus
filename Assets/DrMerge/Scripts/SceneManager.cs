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

    public SceneManager(string pathToA, string pathToB, string sceneFolderPath)
    {
        SceneManager.sceneFolderPath = sceneFolderPath;
        SceneManager.mergedScenePath = sceneFolderPath + "/MergedScene.unity";
        SceneManager.logPath = Application.dataPath + "/DrMerge/Logs";

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
    }

    public void compare()
    {
        DeserializedScene.compare(sceneA, sceneB);

        printMerged(false);
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

            if (!debug)
            {
                //AssetDatabase.Refresh();
                Debug.Log(mergedScenePath);
                EditorApplication.OpenScene(mergedScenePath);
            }
        }
    }

    public void onGUI(Rect editorWindow)
    {
        foreach (Change currentChange in sceneA.changes)
        {
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));

            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));

            currentChange.expanded = EditorGUILayout.Foldout(currentChange.expanded, "Change " + currentChange.nr);

            if (GUILayout.Button("<"))
            {
                currentChange.setChosen(true);

                printMerged(false);
            }

            if (GUILayout.Button(">"))
            {
                currentChange.setChosen(false);

                printMerged(false);
            }

            if (GUILayout.Button("Save"))
            {

            }

            GUILayout.EndHorizontal();

            if (currentChange.expanded)
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

                foreach (long currentID in allIDs)
                {
                    changeColor(new Color(0, 0, 0));

                    GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));

                    resetColorToPrevious();

                    changeColorOnCondition(currentChange.chosen, Color.green, Color.red);

                    GUILayout.BeginVertical(GUILayout.MaxWidth(editorWindow.width / 2));

                    if (sceneA.objects.ContainsKey(currentID))
                    {
                        sceneA.objects[currentID].onGUI();
                    }
                    else
                    {
                        GUILayout.Label("");
                    }

                    GUILayout.EndVertical();

                    resetColorToPrevious();

                    changeColorOnCondition(currentChange.partner.chosen, Color.green, Color.red);

                    if (sceneB.objects.ContainsKey(currentID))
                    {
                        GUILayout.BeginVertical(GUILayout.MaxWidth(editorWindow.width / 2));

                        sceneB.objects[currentID].onGUI();

                        GUILayout.EndVertical();
                    }
                    else
                    {
                        GUILayout.Label("");
                    }

                    resetColorToPrevious();

                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }

    private void changeColorOnCondition(bool condition, Color trueCase, Color falseCase)
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

    private void changeColor(Color desiredColor)
    {
        prevColor = GUI.color;
        GUI.color = desiredColor;
    }

    private void resetColorToPrevious()
    {
        GUI.color = prevColor;
    }
}


