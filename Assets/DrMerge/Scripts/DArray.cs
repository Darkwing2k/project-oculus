using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
                if (!other.references.Contains(this.references[i]))
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
