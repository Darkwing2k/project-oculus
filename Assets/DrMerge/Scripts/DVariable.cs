using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

        if (!name.TrimStart().Equals("- "))
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
        if (value.Contains("fileID"))
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
