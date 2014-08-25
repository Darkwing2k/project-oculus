using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

public class SceneMerger 
{
    public struct Scene
    {
        public string name;
        public string originalPath;

        // there is no Scene class, so the user gives us an Object
        public Object original;
    }

    // the two scenes, which will be merged
    public static Scene a;
    public static Scene b;

    // some path strings, see "getPathsAndNames" for more details
    public static string dataPath;
    public static string mergedScenePath;

    // stored lines of a GameObject from the scene file (for comparison)
    private static List<string> gameObjectLinesA;
    private static List<string> gameObjectLinesB;

    public const string NEWTAG = "[new]";
    public const string DIFFTAG = "[conflict]";

    private static bool reportsActivated;
    private static float percentageDone;
    public delegate void Report(float percentage);
    private static Report reportMethod;

    // -------- new attempt
    private static List<string> sceneA;
    private static List<string> sceneB;
    private static List<string> merged;

    // define needed paths for scenes, merged scene, etc
    public static void getPathsAndNames()
    {
        // --------- define data path (work directory) ------------------
        string[] pieces = Application.dataPath.Split('/');
        dataPath = "";

        // Application.dataPath returns ".../Assets"
        // SceneMerger.dataPath returns ".../"
        // this is due to the fact that the scene paths always start with "/Assets/..."
        // which led to problems with combined absolute paths like ".../AssetsAssets/..."

        for (int i = 0; i < pieces.Length - 1; i++)
        {
            dataPath += pieces[i] + '/';
        }

        // --------- get relative scenes paths --------------------------
        a.originalPath = AssetDatabase.GetAssetPath(a.original);
        b.originalPath = AssetDatabase.GetAssetPath(b.original);

        // --------- get scene names ------------------------------------
        pieces = a.originalPath.Split('/');
        a.name = pieces[pieces.Length - 1].Split('.')[0];

        pieces = b.originalPath.Split('/');
        b.name = pieces[pieces.Length - 1].Split('.')[0];

        // --------- define relative merged-scene path ------------------
        mergedScenePath = "";
        pieces = a.originalPath.Split('/');

        for (int i = 0; i < pieces.Length - 1; i++)
        {
            mergedScenePath += pieces[i] + '/';
        }

        // the merged scene is a txt file at the beginning.
        // it will be changed to a scene file after the merge 
        mergedScenePath += "MergedScene_" + a.name + "_" + b.name + ".txt";

        // store it in util class for other
        DrMergeUtil.absoluteMergedScenePath = dataPath + mergedScenePath;
        DrMergeUtil.relativeMergedScenePath = mergedScenePath;
    }

    // searches the next GameObject in the scene file and stores its lines and id
    private static bool loadNextGameObject(out List<string> lines, out long ID, LineCountReader fin)
    {
        string line = null;
        lines = new List<string>();
        ID = -1;

        while ((line = fin.ReadLine()) != null)
        {
            // found new GameObject. extract id and start to copy lines
            if (line.StartsWith("--- !u!1 "))
            {
                ID = long.Parse(line.Split('&')[1]);

                lines.Add(line);

                while ((line = fin.ReadLine()) != null)
                {
                    // if the beginning of the next GameObject is found, jump back one line
                    if (line.StartsWith("--- !u!1 "))
                    {
                        long lineCountTmp = fin.lineCount;

                        fin.BaseStream.Seek( 0, SeekOrigin.Begin);
                        fin.DiscardBufferedData();
                        fin.lineCount = 0;

                        fin.proceedToLine(lineCountTmp - 1);

                        break;
                    }

                    lines.Add(line);
                }

                return true;
            }
        }

        return false;
    }
       
    // writes GameObject Lines from memory to the merged scene file 
    private static void writeGameObjectToScene(List<string> lines, StreamWriter fout, long ID)
    {
        foreach(string line in lines)
        {
            // stop at the next GameObject mark
            if (line.StartsWith("--- !u!1 ") && !line.EndsWith(ID.ToString()))
            {
                return;
            }
            else
            {
                fout.WriteLine(line);
            }
        }
        
    }
            
    // compares to GameObjects line by line in memory
    private static bool compareGameObjects(List<string> linesA, List<string> linesB, out List<string> merged)
    {
        int iA = 0;
        int iB = 0;
        bool result = true;
        merged = new List<string>();

        while (iA < linesA.Count && iB < linesB.Count)
        {
            // if different lines are found, write them both to the merged scene file
            if (string.Compare(linesA[iA], linesB[iB]) != 0)
            {
                result = false;

                // one of them as a comment (starts with #)
                merged.Add("#" + linesB[iB]);
                merged.Add(linesA[iA]);
            }
            else
            {
                merged.Add(linesA[iA]);
            }

            iA++;
            iB++;
        }

        return result;
    }
          
    // to be able to see the merge result in Unity, it has to be written into the objects name
    private static void addNameTag(List<string> lines, long lineNr, string tag)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains("m_Name"))
            {
                // append "DrMerge", the line number of the GO in the merged scene and a tag (result) to the name.
                // seperated by a unique char
                lines[i] += DrMergeUtil.SEPSYM + "DrMerge" + DrMergeUtil.SEPSYM + lineNr.ToString() + DrMergeUtil.SEPSYM + tag;
            }
        }
    }

    // compares all GameObjects (GOs) of two scenes and writes the merged data to a new one
    public static void performMerge()
    {
        getPathsAndNames();

        using(LineCountReader finA = new LineCountReader(dataPath + a.originalPath))
        using(LineCountReader finB = new LineCountReader(dataPath + b.originalPath))
        using(LineCountWriter fout = new LineCountWriter(dataPath + mergedScenePath))
        {
            // write the beginning of a scene to the merged one
            // this contains RenderSettings, SceneSettings, NavMeshSettings, etc
            // !this has later to be compared too!
            using (LineCountReader fin = new LineCountReader(dataPath + a.originalPath))
            {
                string line = null;

                while ((line = fin.ReadLine()) != null)
                {
                    if (line.StartsWith("--- !u!1 "))
                    {
                        break;
                    }
                    else
                    {
                        fout.WriteLine(line);
                    }
                }
            }

            long idA = 0;
            long idB = 0;

            // load first two GOs into memory
            loadNextGameObject(out gameObjectLinesA, out idA, finA);
            loadNextGameObject(out gameObjectLinesB, out idB, finB);

            while (idA > 0 || idB > 0)
            {
                // GO ids are equal, compare them
                if (idA == idB)
                {
                    List<string> merged;

                    // GOs are identical
                    if (compareGameObjects(gameObjectLinesA, gameObjectLinesB, out merged))
                    {
                        Debug.Log("equal");

                        // write one of them to the merged scene
                        writeGameObjectToScene(merged, fout, idA);

                        // load next two
                        loadNextGameObject(out gameObjectLinesA, out idA, finA);
                        loadNextGameObject(out gameObjectLinesB, out idB, finB);
                    }
                    // GOs are not identical
                    else
                    {
                        Debug.Log("different");

                        // add a tag, so you can see it in unity
                        addNameTag(merged, fout.lineCount + 1, DIFFTAG);

                        // write one of them to the merged scene, with the different variables of the other as comment (was added in compareGameObjects)
                        writeGameObjectToScene(merged, fout, idA);

                        // load next two
                        loadNextGameObject(out gameObjectLinesA, out idA, finA);
                        loadNextGameObject(out gameObjectLinesB, out idB, finB);
                    }
                }
                else
                {
                    // GO ids are not equal
                    // since the GOs are sorted by the id, the smaller one has to be new
                    if (idA > idB)
                    {
                        // A´s id is greater, so B has to be new
                        Debug.Log("only in " + b.name);

                        // add a tag, so you can see it in unity
                        addNameTag(gameObjectLinesB, fout.lineCount + 1, NEWTAG);

                        // write B to the merged scene
                        writeGameObjectToScene(gameObjectLinesB, fout, idB);

                        // load next B (keep A for next comparison)
                        loadNextGameObject(out gameObjectLinesB, out idB, finB);

                        // if this was the last B, all coming As have to be new, so write them all with a tag to the merged scene
                        if (!(idB > 0))
                        {
                            Debug.Log("No more objects in " + b.name);

                            while (idA > 0)
                            {
                                addNameTag(gameObjectLinesA, fout.lineCount + 1, NEWTAG);

                                writeGameObjectToScene(gameObjectLinesA, fout, idA);

                                loadNextGameObject(out gameObjectLinesA, out idA, finA);
                            }
                        }
                    }
                    else
                    {
                        // B´s id is greater, so A has to be new
                        // do the same thing as above, just vice versa
                        Debug.Log("only in " + a.name);

                        addNameTag(gameObjectLinesA, fout.lineCount + 1, NEWTAG);

                        writeGameObjectToScene(gameObjectLinesA, fout, idA);

                        loadNextGameObject(out gameObjectLinesA, out idA, finA);

                        if (!(idA > 0))
                        {
                            Debug.Log("No more objects in " + a.name);

                            while (idB > 0)
                            {
                                addNameTag(gameObjectLinesB, fout.lineCount + 1, NEWTAG);

                                writeGameObjectToScene(gameObjectLinesB, fout, idB);

                                loadNextGameObject(out gameObjectLinesB, out idB, finB);
                            }
                        }
                    }
                }
            }
        }
        // all GOs are compared. change the file ending from ".txt" to ".unity" to get a scene file
        File.Move(dataPath + mergedScenePath, Path.ChangeExtension(dataPath + mergedScenePath, ".unity"));
        mergedScenePath = Path.ChangeExtension(mergedScenePath, ".unity");
        
        // store it in the util class for other
        DrMergeUtil.absoluteMergedScenePath = dataPath + mergedScenePath;
        DrMergeUtil.relativeMergedScenePath = mergedScenePath;

        // clear memory of read lines
        gameObjectLinesA.Clear();
        gameObjectLinesB.Clear();
    }

    // ------- new attempt
    public static void activateProgressReports(Report reportMethod)
    {
        reportsActivated = true;
        SceneMerger.reportMethod = reportMethod;
    }

    private static List<string> fetchAllSceneLines(string path)
    {
        List<string> result = new List<string>();

        using (StreamReader fin = new StreamReader(path))
        {
            
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                result.Add(line);
            }
        }

        return result;
    }

    private static long getNextGoID(List<string> lines, int start, out int index)
    {
        for (int i = start; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("--- !u!1 "))
            {
                index = i;

                return long.Parse(lines[i].Split('&')[1]);
            }
        }

        index = -1;
        return -1;
    }

    public static void runComparison()
    {
        sceneA = fetchAllSceneLines(dataPath + a.originalPath);
        sceneB = fetchAllSceneLines(dataPath + b.originalPath);

        merged = new List<string>();

        int currentLineA = 0;
        int currentLineB = 0;

        // copy the beginning
        for (int i = 0; i < sceneA.Count; i++)
        {
            if (sceneA[i].StartsWith("--- !u!1 "))
            {
                currentLineA = i;
                break;
            }
            else
            {
                merged.Add(sceneA[i]);
            }
        }

        // for the progress bar
        if (reportsActivated)
        {
            percentageDone = (float)(currentLineA + currentLineB) / (float)(sceneA.Count + sceneB.Count);
            reportMethod(percentageDone);
        }

        long idA = getNextGoID(sceneA, 0, out currentLineA);
        long idB = getNextGoID(sceneB, 0, out currentLineB);

        while (idA > 0 || idB > 0)
        {
            if (idA == idB)
            {
                bool equal = true;
                int goIndex = merged.Count + 1;

                int currLineATmp = currentLineA;
                int currLineBTmp = currentLineB;

                List<int> relativeDiffIndexes = new List<int>();

                while (currLineATmp < sceneA.Count && currLineBTmp < sceneB.Count)
                {
                    bool endA = sceneA[currLineATmp].StartsWith("--- !u!1 ") && !sceneA[currLineATmp].EndsWith(idA.ToString());
                    bool endB = sceneB[currLineBTmp].StartsWith("--- !u!1 ") && !sceneB[currLineBTmp].EndsWith(idB.ToString());

                    if (endA || endB)
                    {
                        if (endA && endB)
                        {
                            break;
                        }
                        else if (endA)
                        {
                            //merged.Add("#");
                            //merged.Add(sceneA[currentLineA]);

                            equal = false;

                            relativeDiffIndexes.Add(-1);
                            relativeDiffIndexes.Add(currLineBTmp - currentLineB);

                            currLineBTmp++;
                            //currentLineA++;
                        }
                        else if (endB)
                        {
                            //merged.Add('#' + sceneB[currentLineB]);
                            //merged.Add("");

                            equal = false;

                            relativeDiffIndexes.Add(currLineATmp - currentLineA);
                            relativeDiffIndexes.Add(-1);

                            currLineATmp++;
                            //currentLineB++;
                        }
                    }
                    else
                    {
                        if (string.Compare(sceneA[currLineATmp], sceneB[currLineBTmp]) == 0)
                        {
                            //merged.Add(sceneA[currentLineA]);

                            currLineATmp++;
                            currLineBTmp++;
                        }
                        else
                        {
                            //merged.Add('#' + sceneB[currentLineB]);
                            //merged.Add(sceneA[currentLineA]);

                            equal = false;

                            string propertyA = sceneA[currLineATmp].Split(':')[0];
                            string propertyB = sceneB[currLineBTmp].Split(':')[0];

                            if (propertyA != null && propertyB != null && string.Compare(propertyA, propertyB) == 0)
                            {
                                relativeDiffIndexes.Add(currLineATmp - currentLineA);
                                relativeDiffIndexes.Add(currLineBTmp - currentLineB);

                                currLineATmp++;
                                currLineBTmp++;
                            }
                            else
                            {
                                int indexP = isThisPropertyComing(sceneB, currLineBTmp, propertyA);

                                if (indexP > 0)
                                {
                                    relativeDiffIndexes.Add(currLineATmp - currentLineA);
                                    relativeDiffIndexes.Add(indexP - currentLineB);

                                    currLineATmp++;
                                }
                                else
                                {
                                    relativeDiffIndexes.Add(currLineATmp - currentLineA);
                                    relativeDiffIndexes.Add(-1);

                                    currLineATmp++;
                                }
                            }
                        }
                    }
                }

                /*
                if (!equal)
                {
                    Debug.Log("different");

                    for (int i = merged.Count - 1; i >= goIndex; i--)
                    {
                        if (merged[i].Contains("m_Name"))
                        {
                            merged[i] += DrMergeUtil.SEPSYM + "DrMerge" + DrMergeUtil.SEPSYM + goIndex.ToString() + DrMergeUtil.SEPSYM + DIFFTAG;
                        }
                    }
                }
                */

                

                while (currentLineA < currLineATmp)
                {
                    if (!equal)
                    {
                        if (sceneA[currentLineA].TrimStart().StartsWith("m_Name"))
                        {
                            sceneA[currentLineA] += DrMergeUtil.SEPSYM + "DrMerge" + DrMergeUtil.SEPSYM + goIndex.ToString() + DrMergeUtil.SEPSYM + DIFFTAG;
                        }
                    }

                    merged.Add(sceneA[currentLineA]);
                    currentLineA++;
                }

                if (!equal)
                {
                    string infoLine = "#";

                    foreach (int i in relativeDiffIndexes)
                    {
                        infoLine += DrMergeUtil.SEPSYM + i.ToString();
                    }

                    merged.Add(infoLine);

                    while (currentLineB < currLineBTmp)
                    {
                        if (sceneB[currentLineB].TrimStart().StartsWith("m_Name"))
                        {
                            sceneB[currentLineB] += DrMergeUtil.SEPSYM + "DrMerge" + DrMergeUtil.SEPSYM + goIndex.ToString() + DrMergeUtil.SEPSYM + DIFFTAG;
                        }

                        merged.Add("#" + sceneB[currentLineB]);
                        currentLineB++;
                    }
                }

                idA = getNextGoID(sceneA, currentLineA - 1, out currentLineA);
                idB = getNextGoID(sceneB, currentLineB - 1, out currentLineB);
            }
            else
            {
                if (idA > idB)
                {
                    Debug.Log("new from " + b.name);

                    currentLineB = addAsNew(sceneB, currentLineB, idB);

                    idB = getNextGoID(sceneB, currentLineB - 1, out currentLineB);

                    if (!(idB > 0))
                    {
                        while (idA > 0)
                        {
                            currentLineA = addAsNew(sceneA, currentLineA, idA);

                            idA = getNextGoID(sceneA, currentLineA - 1, out currentLineA);
                        }
                    }
                }
                else
                {
                    Debug.Log("new from " + a.name);

                    currentLineA = addAsNew(sceneA, currentLineA, idA);

                    idA = getNextGoID(sceneA, currentLineA - 1, out currentLineA);

                    if (!(idA > 0))
                    {
                        while (idB > 0)
                        {
                            currentLineB = addAsNew(sceneB, currentLineB, idB);

                            idB = getNextGoID(sceneB, currentLineB - 1, out currentLineB);
                        }
                    }
                }
            }

            float cLineA = currentLineA > 0 ? currentLineA : sceneA.Count;
            float cLineB = currentLineB > 0 ? currentLineB : sceneB.Count;

            if (reportsActivated)
            {
                percentageDone = (cLineA + cLineB) / (float)(sceneA.Count + sceneB.Count);
                reportMethod(percentageDone);
            }
        }

        using (StreamWriter fout = new StreamWriter(dataPath + mergedScenePath))
        {
            for (int i = 0; i < merged.Count; i++)
            {
                fout.WriteLine(merged[i]);
            }
        }

        // all GOs are compared. change the file ending from ".txt" to ".unity" to get a scene file
        File.Move(dataPath + mergedScenePath, Path.ChangeExtension(dataPath + mergedScenePath, ".unity"));
        mergedScenePath = Path.ChangeExtension(mergedScenePath, ".unity");

        // store it in the util class for other
        DrMergeUtil.absoluteMergedScenePath = dataPath + mergedScenePath;
        DrMergeUtil.relativeMergedScenePath = mergedScenePath;

        sceneA.Clear();
        sceneB.Clear();
        merged.Clear();
    }

    private static int addAsNew(List<string> scene, int currentLine, long id)
    {
        int goIndex = merged.Count + 1;

        while (currentLine < scene.Count)
        {
            if (scene[currentLine].Contains("--- !u!1 ") && !scene[currentLine].EndsWith(id.ToString()))
            {
                break;
            }
            else if (scene[currentLine].TrimStart().StartsWith("m_Name"))
            {
                merged.Add(scene[currentLine] + DrMergeUtil.SEPSYM + "DrMerge" + DrMergeUtil.SEPSYM + goIndex.ToString() + DrMergeUtil.SEPSYM + NEWTAG);
            }
            else
            {
                merged.Add(scene[currentLine]);
            }

            currentLine++;
        }

        return currentLine;
    }

    private static int isThisPropertyComing(List<string> lines, int index, string searchedProperty)
    {
        for (int i = index + 1; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith("--- !u!") || lines[i].TrimStart().StartsWith(DrMergeUtil.SEPSYM + ""))
            {
                break;
            }

            string currProperty = lines[i].Split(':')[0];

            if (currProperty == null)
                continue;

            currProperty = currProperty.TrimStart();

            if (string.Compare(searchedProperty, currProperty) == 0)
            {
                return i;
            }
        }

        return -1;
    }
}
