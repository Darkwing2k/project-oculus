using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DeserializedScene
{
    public static SceneManager manager;

    public string name;
    public int parseIndex;
    public int objectCount;

    public List<string> allSceneLines;

    public Dictionary<long, DObject> objects;

    public List<Change> changes;

    public static Color prevColor;

    public DeserializedScene(SceneManager manager, List<string> allLines)
    {
        DeserializedScene.manager = manager;

        allSceneLines = new List<string>();

        for (int i = 0; i < allLines.Count; i++)
        {
            allSceneLines.Add(allLines[i]);
        }

        parseIndex = 2;
        objectCount = 0;

        objects = new Dictionary<long, DObject>();
        changes = new List<Change>();
    }

    public static void compare(DeserializedScene sceneA, DeserializedScene sceneB)
    {
        foreach (DObject currentObjA in sceneA.objects.Values)
        {
            if (sceneB.objects.ContainsKey(currentObjA.id))
            {
                if (currentObjA.isEqualTo(sceneB.objects[currentObjA.id]))
                {
                    currentObjA.result = DObject.CompareResult.EQUAL;
                    sceneB.objects[currentObjA.id].result = DObject.CompareResult.EQUAL;

                    currentObjA.chosen = true;
                    sceneB.objects[currentObjA.id].chosen = false;
                }
                else
                {
                    currentObjA.result = DObject.CompareResult.DIFFERENT;
                    sceneB.objects[currentObjA.id].result = DObject.CompareResult.DIFFERENT;
                }
            }
            else
            {
                currentObjA.result = DObject.CompareResult.NEW;
            }
        }

        foreach (DObject currentObjB in sceneB.objects.Values)
        {
            if (currentObjB.result == DObject.CompareResult.UNDECIDED)
            {
                currentObjB.result = DObject.CompareResult.NEW;
            }
        }

        sceneA.linkObjectsInChange(true);
        sceneB.linkObjectsInChange(false);

        linkChanges(sceneA, sceneB);
    }

    private void linkObjectsInChange(bool changeIsChosen)
    {
        foreach (DObject currentObject in objects.Values)
        {
            if (currentObject.result != DObject.CompareResult.EQUAL && currentObject.change == null)
            {
                Change ch = new Change(this, changeIsChosen);

                ch.references = currentObject.getReferencesOfDiffs();

                ch.references.Add(currentObject.id);

                foreach (long currentID in ch.references)
                {
                    objects[currentID].change = ch;
                }

                ch.updateChosenInRefs();

                changes.Add(ch);
            }
        }
    }

    private static void linkChanges(DeserializedScene sceneA, DeserializedScene sceneB)
    {
        foreach (DObject currentObjA in sceneA.objects.Values)
        {
            if (currentObjA.result == DObject.CompareResult.DIFFERENT)
            {
                DObject currentObjB = sceneB.objects[currentObjA.id];

                currentObjA.change.partner = currentObjB.change;
                currentObjB.change.partner = currentObjA.change;
            }
        }
    }

    public void parse()
    {
        while (parseIndex < allSceneLines.Count)
        {
            DObject tmpO = new DObject(objectCount);

            objectCount++;

            tmpO.parse(allSceneLines, ref parseIndex);

            objects.Add(tmpO.id, tmpO);
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

        foreach (DObject current in objects.Values)
        {
            current.print(fout, merged, debug);
        }
    }
}

public abstract class Deserialized
{
    public string name;

    public abstract bool isEqualTo(Deserialized other);

    public abstract void parse(List<string> allSceneLines, ref int index);

    public abstract List<long> getReferencesOfDiffs();

    //public abstract void addReferencesToChange(Change ch);

    public abstract void print(StreamWriter fout, bool merged, bool debug);

    public abstract void onGUI();
}

public class DObject : Deserialized
{
    public long id;
    public int type;
    public string typeName;
    public int number;

    public Change change;

    public List<Deserialized> members;
    public List<int> differences;

    public enum CompareResult { UNDECIDED, EQUAL, DIFFERENT, NEW };
    public CompareResult result;

    public bool expanded;
    public bool chosen;

    public DObject(int n)
    {
        name = "";
        number = n;

        members = new List<Deserialized>();
        differences = new List<int>();

        result = CompareResult.UNDECIDED;
    }

    public override bool isEqualTo(Deserialized d)
    {
        DObject other = (DObject)d;

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

                members.Add(tmpV);
            }

            index++;
        }
    }

    public override List<long> getReferencesOfDiffs()
    {
        List<long> refs = new List<long>();

        foreach (int i in differences)
        {
            refs.AddRange(members[i].getReferencesOfDiffs());
        }

        return refs;
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

                if (result != CompareResult.EQUAL)
                {
                    fout.Write("Referencing: ");
                    foreach (long ID in change.references)
                    {
                        fout.Write(ID + " ");
                    }
                    fout.WriteLine();

                    fout.WriteLine("Change Nr: " + change.nr);

                    fout.WriteLine("Change Partner: " + change.partner.nr);
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
        string textToDisplay = (name + " (" + typeName + ")" + " (" + result + ")").TrimStart();

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
        else if (result == CompareResult.NEW)
        {
            GUILayout.Label(textToDisplay);
        }
    }
}

public class DVariable : Deserialized
{
    public string value;

    public long reference;

    public override bool isEqualTo(Deserialized m)
    {
        DVariable other = (DVariable)m;

        if (string.Compare(name, other.name) != 0)
        {
            return false;
        }

        if (string.Compare(value, other.value) != 0)
        {
            return false;
        }

        return true;
    }

    public override void parse(List<string> allSceneLines, ref int index)
    {
        string line = allSceneLines[index];

        if (line.Contains("{"))
        {
            if (line.IndexOf('{') < line.IndexOf(':'))
            {
                char[] sep = { '{' };

                string[] pieces = line.Split(sep, 2);

                this.name = pieces[0];
                this.value = "{" + pieces[1];

                reference = extractID();

                return;
            }
        }

        char[] sep2 = { ':' };

        string[] pieces2 = line.Split(sep2, 2);

        this.name = pieces2[0];
        this.value = pieces2[1];

        reference = extractID();
    }

    public override List<long> getReferencesOfDiffs()
    {
        List<long> refs = new List<long>();

        if (reference > 0)
        {
            refs.Add(reference);
        }

        return refs;
    }

    public override void print(StreamWriter fout, bool merged, bool debug)
    {
        string lineToWrite = "";

        if (debug)
        {
            lineToWrite += "Variable:: ";
        }

        lineToWrite += name;

        if (! name.TrimStart().Equals("- "))
        {
            lineToWrite += ":";
        }

        lineToWrite += value;

        if (debug)
        {
            if (reference != -1)
            {
                lineToWrite += " " + reference;
            }
        }

        fout.WriteLine(lineToWrite);
    }

    public override void onGUI()
    {
        GUILayout.Label((name + ": " + value).TrimStart());
    }

    public long extractID()
    {
        if(value.Contains("fileID"))
        {
            string[] sep = { "fileID" };

            string idString = value.Split(sep, StringSplitOptions.None)[1];

            int end = 2;

            while (end < idString.Length)
            {
                if (!(idString[end] >= '0' && idString[end] <= '9'))
                {
                    break;
                }
                end++;
            }

            return long.Parse(idString.Substring(2, end - 2));
        }
        
        return -1;
    }
}

public class DArray : Deserialized
{
    public List<DVariable> variables;

    public List<long> references;

    public List<int> differences;

    public bool expanded;

    public DArray(string n)
    {
        name = n;
        variables = new List<DVariable>();
        references = new List<long>();
        differences = new List<int>();
    }

    public override bool isEqualTo(Deserialized m)
    {
        DArray other = (DArray)m;

        bool equal = true;

        if (string.Compare(name, other.name) != 0)
        {
            equal = false;
        }

        if (variables.Count != other.variables.Count)
        {
            equal = false;
        }

        if (references.Count == 0)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (!variables[i].isEqualTo(other.variables[i]))
                {
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < references.Count; i++)
            {
                if (! other.references.Contains(this.references[i]))
                {
                    differences.Add(i);
                    equal = false;
                }
            }

            for (int i = 0; i < other.references.Count; i++)
            {
                if (!this.references.Contains(other.references[i]))
                {
                    other.differences.Add(i);
                    equal = false;
                }
            }
        }

        return equal;
    }

    public override void parse(List<string> allSceneLines, ref int index)
    {
        string line = allSceneLines[index];

        int offset = DArray.countWhitespaces(line);

        index++;

        while (offset == DArray.countWhitespaces(allSceneLines[index]) - 1)
        {
            DVariable tmp = new DVariable();

            tmp.parse(allSceneLines, ref index);

            variables.Add(tmp);

            index++;
        }

        index--;

        foreach (DVariable current in variables)
        {
            if (current.reference >= 0)
            {
                references.Add(current.reference);
            }
        }
    }

    public override List<long> getReferencesOfDiffs()
    {
        List<long> refs = new List<long>();

        foreach (int i in differences)
        {
            refs.Add(references[i]);
        }

        return refs;
    }

    public override void print(StreamWriter fout, bool merged, bool debug)
    {
        string lineToWrite = "";

        if (debug)
        {
            lineToWrite += "Array   :: ";
        }

        lineToWrite += name + ":";

        if (variables.Count == 0)
        {
            lineToWrite += " []";
        }

        if (debug)
        {
            foreach (long current in references)
            {
                if (current != -1)
                {
                    lineToWrite += " " + current;
                }
            }
        }

        fout.WriteLine(lineToWrite);

        for (int i = 0; i < variables.Count; i++)
        {
            if (debug && differences.Contains(i))
            {
                fout.Write("DIFF: ");
            }

            variables[i].print(fout, merged, debug);
        }
    }

    public override void onGUI()
    {
        expanded = EditorGUILayout.Foldout(expanded, (name).TrimStart());

        if (expanded)
        {
            foreach (DVariable current in variables)
            {
                current.onGUI();
            }
        }
    }

    private static int countWhitespaces(string line)
    {
        int result = 0;

        for (int i = 0; i + 1 < line.Length; i += 2)
        {
            if (line[i] == ' ' && line[i + 1] == ' ' || line[i] == '-' && line[i + 1] == ' ')
            {
                result++;
            }
            else
            {
                break;
            }
        }

        return result;
    }
}
