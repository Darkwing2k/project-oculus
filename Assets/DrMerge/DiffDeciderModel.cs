using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DiffDeciderModel
{
    private List<string> allSceneLines;

    private List<Entry> allEntries;

    private List<GoEntryNew> newEntries;
    private List<GoEntryDiff> diffEntries;

    private List<GoEntryNew> deleteLaterNew;
    private List<GoEntryDiff> deleteLaterDiff;

    public int NewEntriesCount
    {
        get { return newEntries.Count; }
    }

    public int DiffEntriesCount
    {
        get { return diffEntries.Count; }
    }

    public DiffDeciderModel()
    {
        allSceneLines = new List<string>();
        allEntries = new List<Entry>();

        newEntries = new List<GoEntryNew>();
        diffEntries = new List<GoEntryDiff>();

        deleteLaterNew = new List<GoEntryNew>();
        deleteLaterDiff = new List<GoEntryDiff>();

        using (StreamReader fin = new StreamReader(DrMergeUtil.absoluteMergedScenePath))
        {
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                allSceneLines.Add(line);
            }
        }

        GameObject[] allGos = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject currentGo in allGos)
        {
            // handling new gameobjects
            if (currentGo.name.EndsWith(SceneMerger.NEWTAG))
            {
                string[] pieces = currentGo.name.Split(DrMergeUtil.SEPSYM);

                int indexS = int.Parse(pieces[pieces.Length - 2]) - 1;
                int indexE = indexS;
                int indexN = indexS;

                for (int i = indexS + 1; i < allSceneLines.Count; i++)
                {
                    if (allSceneLines[i].StartsWith("--- !u!1 "))
                    {
                        indexE = i - 1;
                        break;
                    }
                    else if (allSceneLines[i].TrimStart().StartsWith("m_Name"))
                    {
                        indexN = i;
                    }
                }

                GoEntryNew currentGoEntryNew = new GoEntryNew(this, currentGo.name, indexS, indexE, indexN);

                newEntries.Add(currentGoEntryNew);
                allEntries.Add(currentGoEntryNew);
            }

            // handling different gameobjects
            if (currentGo.name.EndsWith(SceneMerger.DIFFTAG))
            {
                string[] pieces = currentGo.name.Split(DrMergeUtil.SEPSYM);

                int indexS = int.Parse(pieces[pieces.Length - 2]) - 1;
                int indexE = indexS;
                int indexN = indexS;
                int indexN2 = indexS;
                List<DiffEntry> diffs = new List<DiffEntry>();

                int commentStart = -1;

                for (int i = indexS + 1; i < allSceneLines.Count; i++)
                {
                    if(allSceneLines[i].StartsWith("#"))
                    {
                        /*
                        DiffEntry tmp = new DiffEntry(this, i, i + 1, allSceneLines[i + 1], allSceneLines[i].Split('#')[1]);

                        diffs.Add(tmp);
                        allEntries.Add(tmp);
                        */

                        if(commentStart == -1)
                            commentStart = i;

                        if(allSceneLines[i].Contains("m_Name"))
                        {
                            indexN2 = i - 1;
                            break;
                        }
                    }
                    else if (allSceneLines[i].TrimStart().StartsWith("m_Name"))
                    {
                        indexN = i;
                    }
                }

                string[] diffLines = allSceneLines[commentStart].Split(DrMergeUtil.SEPSYM);

                for (int i = 1; i < diffLines.Length; i += 2)
                {
                    int indexA = int.Parse(diffLines[i]);
                    int indexB = int.Parse(diffLines[i+1]);

                    string A = "";
                    string B = "";

                    if(indexA > 0)
                        A = allSceneLines[indexS + indexA];

                    if(indexB > 0)
                        B = allSceneLines[commentStart + 1 + indexB];

                    DiffEntry tmp = new DiffEntry(this, indexA, indexB, A, B);

                    diffs.Add(tmp);
                    allEntries.Add(tmp);
                }

                allSceneLines.Remove(allSceneLines[commentStart]);

                for (int i = commentStart; i < allSceneLines.Count; i++)
                {
                    if (allSceneLines[i].StartsWith("--- !u!1 "))
                    {
                        indexE = i;
                        break;
                    }
                }

                GoEntryDiff currentGoEntryDiff = new GoEntryDiff(this, diffs, currentGo.name, indexS, commentStart, indexE, indexN, indexN2);

                diffEntries.Add(currentGoEntryDiff);
                allEntries.Add(currentGoEntryDiff);
            }
        }
        writeToScene();
    }

    // update line numbers of all entries if for example a gameobject was deleted
    private void triggerLinesChanged(int from, int offset)
    {
        foreach (Entry current in allEntries)
        {
            current.updateLineNrs(from, offset);
        }
    }

    private void writeToScene()
    {
        using (StreamWriter fout = new StreamWriter(DrMergeUtil.absoluteMergedScenePath, false))
        {
            foreach (string line in allSceneLines)
            {
                fout.WriteLine(line);
            }
        }

        EditorApplication.OpenScene(DrMergeUtil.relativeMergedScenePath);
    }

    public void removeLines(int from, int to)
    {
        for (int i = to; i >= from; i--)
        {
            allSceneLines.RemoveAt(i);
        }

        triggerLinesChanged(from, from - to - 1);

        writeToScene();
    }

    public void changeName(int at, string newName, bool directlyWriteToScene)
    {
        Debug.Log(allSceneLines[at]);

        if (allSceneLines[at].TrimStart().StartsWith("m_Name"))
        {
            string[] pieces = allSceneLines[at].Split(':');

            allSceneLines[at] = pieces[0] + ": " + newName;

            if (directlyWriteToScene)
                writeToScene();
        }
    }

    public void changeComment(int at, string commented, string uncommented)
    {
        allSceneLines[at] = "#" + commented;
        allSceneLines[at + 1] = uncommented;

        writeToScene();
    }

    public void changeComment(int removeFrom, int removeTo, int addFrom, int addTo)
    {
        for (int i = removeFrom; i < removeTo; i++)
        {
            allSceneLines[i] = allSceneLines[i].Split('#')[1];
        }

        for (int i = addFrom; i < addTo; i++)
        {
            allSceneLines[i] = "#" + allSceneLines[i];
        }

        writeToScene();
    }

    public void removeComments(int from, int to)
    {
        int removedLines = 0;

        for (int i = to; i >= from; i--)
        {
            if (allSceneLines[i].StartsWith("#"))
            {
                removedLines--;
                allSceneLines.RemoveAt(i);
            }
        }

        triggerLinesChanged(from, removedLines);

        writeToScene();
    }

    public void newGoOnGUI()
    {
        EditorGUILayout.BeginVertical();

        foreach (GoEntryNew current in newEntries)
        {
            current.onGUI();
        }

        EditorGUILayout.EndVertical();

        foreach (GoEntryNew current in deleteLaterNew)
        {
            newEntries.Remove(current);
            allEntries.Remove(current);
        }

        deleteLaterNew.Clear();
    }

    public void diffGoOnGUI()
    {
        foreach (GoEntryDiff current in diffEntries)
        {
            current.onGUI();
        }

        foreach (GoEntryDiff current in deleteLaterDiff)
        {
            diffEntries.Remove(current);
            allEntries.Remove(current);
        }

        deleteLaterDiff.Clear();
    }

    public void removeMe(GoEntryNew me)
    {
        deleteLaterNew.Add(me);
    }

    public void removeMe(GoEntryDiff me)
    {
        deleteLaterDiff.Add(me);
    }
}
