using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DComponent : Deserialized
{
    public long id;
    public int type;
    public string typeName;
    public int number;

    public DeserializedScene scene;

    public List<Deserialized> members;
    public List<int> differences;

    public long gameObjectReference;
    public HashSet<long> references;
    public int changeNr;

    public enum CompareResult { UNDECIDED, EQUAL, DIFFERENT, NEW };
    public CompareResult result;

    public bool expanded;
    public bool chosen;

    public DComponent(DeserializedScene scene, int nr)
    {
        name = "";
        number = nr;

        this.scene = scene;

        members = new List<Deserialized>();
        differences = new List<int>();
        references = new HashSet<long>();

        result = CompareResult.UNDECIDED;
        changeNr = -1;
        gameObjectReference = -1;
    }

    public override bool isEqualTo(Deserialized d)
    {
        DComponent other = (DComponent)d;

        bool equal = true;

        for (int i = 0; i < members.Count; i++)
        {
            if (!members[i].isEqualTo(other.members[i]))
            {
                equal = false;
                differences.Add(i);
                other.differences.Add(i);
            }
        }

        return equal;
    }

    public override void parse(List<string> allSceneLines, ref int index)
    {
        string[] typeAndIdLine = allSceneLines[index].Split(' ');

        typeAndIdLine[1] = typeAndIdLine[1].Split('!')[2];

        typeAndIdLine[2] = typeAndIdLine[2].Split('&')[1];

        id = long.Parse(typeAndIdLine[2]);
        type = int.Parse(typeAndIdLine[1]);

        index++;

        typeName = allSceneLines[index].Split(':')[0];

        index++;

        string line;

        while (index < allSceneLines.Count && !(line = allSceneLines[index]).StartsWith("--- !u!"))
        {
            if (line.TrimStart().StartsWith("m_Name:"))
            {
                name = line.Split(':')[1];

                allSceneLines[index] = line + SceneManager.SEPSYM + id;
            }

            if (line.EndsWith(":") || line.EndsWith("[]"))
            {
                DArray tmpA = new DArray(line.Split(':')[0]);

                tmpA.parse(allSceneLines, ref index);

                members.Add(tmpA);
            }
            else
            {
                DVariable tmpV = new DVariable();

                tmpV.parse(allSceneLines, ref index);

                if (type != 1 && tmpV.name.TrimStart().StartsWith("m_GameObject"))
                {
                    gameObjectReference = tmpV.reference;
                }

                members.Add(tmpV);
            }

            index++;
        }
    }

    public override void print(StreamWriter fout, bool merged, bool debug)
    {
        /*
        if (debug && result != CompareResult.EQUAL)
        {
            fout.WriteLine("Change: " + change.myNr + " connected to " + change.partner.myNr);
        }
        */

        if (!merged || chosen) // merged -> chosen (wenn merged, dann chosen)
        {
            fout.WriteLine("--- !u!" + type + " &" + id);

            if (debug)
            {
                fout.WriteLine("Object Nr: " + number);
                fout.WriteLine("Status: " + result);

                fout.Write("RefSet: ");
                foreach (long ID in references)
                {
                    fout.Write(ID + " ");
                }
                fout.WriteLine();

                if (result != CompareResult.EQUAL)
                {
                    if (type != 1)
                    {
                        fout.WriteLine("GameObjectRef: " + gameObjectReference);
                    }

                    fout.WriteLine("ChangeNr: " + changeNr);
                }
            }

            fout.WriteLine(typeName + ":");

            for (int i = 0; i < members.Count; i++)
            {
                if (debug && differences.Contains(i))
                {
                    fout.Write("DIFF: ");
                }

                members[i].print(fout, merged, debug);
            }
        }
    }

    public override void onGUI()
    {
        string textToDisplay = (name + " (" + typeName + ")").TrimStart();

        if (result == CompareResult.DIFFERENT)
        {
            expanded = EditorGUILayout.Foldout(expanded, textToDisplay);

            EditorGUILayout.BeginVertical();
            if (expanded)
            {
                foreach (int i in differences)
                {
                    members[i].onGUI();
                }
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            GUILayout.Label(textToDisplay);
        }
    }

    public void fillRefSet()
    {
        List<long> allIDs = getReferencesOfDiffs();

        references.Add(this.id);

        foreach (long ID in allIDs)
        {
            references.Add(ID);
        }

        foreach (long ID in references)
        {
            if(scene.components.ContainsKey(ID) && ID != this.id)
            {
                scene.components[ID].addRefs(references);
            }

            if (scene.opponent.components.ContainsKey(ID))
            {
                scene.opponent.components[ID].addRefs(references);
            }
        }
    }

    public override List<long> getReferencesOfDiffs()
    {
        List<long> refs = new List<long>();

        if (gameObjectReference > 0)
        {
            refs.Add(gameObjectReference);
        }

        foreach (int i in differences)
        {
            refs.AddRange(members[i].getReferencesOfDiffs());
        }

        return refs;
    }

    public void addRefs(HashSet<long> refs)
    {
        foreach (long ID in refs)
        {
            references.Add(ID);
        }
    }

    public void setChangeNr(int i)
    {
        changeNr = i;

        foreach (long ID in references)
        {
            if (scene.components.ContainsKey(ID) && ID != this.id)
            {
                scene.components[ID].changeNr = i;
            }

            if (scene.opponent.components.ContainsKey(ID))
            {
                scene.opponent.components[ID].changeNr = i;
            }
        }
    }
}
