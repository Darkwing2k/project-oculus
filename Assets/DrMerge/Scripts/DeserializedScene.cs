using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DeserializedScene
{
    public static SceneManager sceneManager;
    public DeserializedScene opponent;

    public string name;
    public int parseIndex;
    public int objectCount;
    public static int changeCount = 0;

    public List<string> allSceneLines;

    public Dictionary<long, DComponent> components;

    public DeserializedScene(SceneManager manager, List<string> allLines)
    {
        DeserializedScene.sceneManager = manager;

        allSceneLines = new List<string>();

        for (int i = 0; i < allLines.Count; i++)
        {
            allSceneLines.Add(allLines[i]);
        }

        parseIndex = 2;
        objectCount = 0;

        components = new Dictionary<long, DComponent>();
    }

    public static void compare(DeserializedScene sceneA, DeserializedScene sceneB)
    {
        foreach (DComponent currentCmpA in sceneA.components.Values)
        {
            if (sceneB.components.ContainsKey(currentCmpA.id))
            {
                DComponent currentObjB = sceneB.components[currentCmpA.id];

                if (currentCmpA.isEqualTo(sceneB.components[currentCmpA.id]))
                {
                    currentCmpA.result = DComponent.CompareResult.EQUAL;
                    currentObjB.result = DComponent.CompareResult.EQUAL;

                    currentCmpA.chosen = true;
                    currentObjB.chosen = false;
                }
                else
                {
                    currentCmpA.result = DComponent.CompareResult.DIFFERENT;
                    currentObjB.result = DComponent.CompareResult.DIFFERENT;
                }
            }
            else
            {
                currentCmpA.result = DComponent.CompareResult.NEW;
            }
        }

        foreach (DComponent currentCmpB in sceneB.components.Values)
        {
            if (currentCmpB.result == DComponent.CompareResult.UNDECIDED)
            {
                currentCmpB.result = DComponent.CompareResult.NEW;
            }
        }
    }

    public void parse()
    {
        while (parseIndex < allSceneLines.Count)
        {
            DComponent tmpO = new DComponent(this, objectCount);

            objectCount++;

            tmpO.parse(allSceneLines, ref parseIndex);

            components.Add(tmpO.id, tmpO);
        }
    }

    private void printHead(StreamWriter fout)
    {
        fout.WriteLine(allSceneLines[0]);
        fout.WriteLine(allSceneLines[1]);
    }

    public void print(StreamWriter fout, bool debug)
    {
        print(fout, false, true, debug);
    }

    public void print(StreamWriter fout, bool merged, bool includeHead, bool debug)
    {
        if (includeHead)
        {
            printHead(fout);
        }

        foreach (DComponent current in components.Values)
        {
            current.print(fout, merged, debug);
        }
    }

    public void fillRefSets()
    {
        foreach (DComponent currentCmp in components.Values)
        {
            if (currentCmp.result != DComponent.CompareResult.EQUAL)
            {
                currentCmp.fillRefSet();
            }
        }
    }

    public void getAllIDsOfChangedComponents(HashSet<long> setOfIDs)
    {
        foreach (DComponent currentCmp in components.Values)
        {
            if (currentCmp.references.Count > 0 && currentCmp.changeNr == -1)
            {
                foreach (long ID in currentCmp.references)
                {
                    setOfIDs.Add(ID);
                }

                currentCmp.setChangeNr(changeCount);
                changeCount++;
            }
        }
    }
}
