using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DeserializedScene
{
    public string name;

    public int index;

    public List<string> allSceneLines;

    public Dictionary<long, DeserializedObject> objects;

    public DeserializedScene(List<string> allLines)
    {
        allSceneLines = new List<string>();

        for (int i = 0; i < allLines.Count; i++)
        {
            allSceneLines.Add(allLines[i]);
        }

        index = 2;
        objects = new Dictionary<long, DeserializedObject>();
    }

    public void compare(DeserializedScene otherScene)
    {
        foreach (KeyValuePair<long, DeserializedObject> current in objects)
        {
            if (otherScene.objects.ContainsKey(current.Key))
            {
                if (current.Value.equal(otherScene.objects[current.Key]))
                {
                    current.Value.result = DeserializedObject.CompareResult.EQUAL;
                    otherScene.objects[current.Key].result = DeserializedObject.CompareResult.EQUAL;
                }
                else
                {
                    current.Value.result = DeserializedObject.CompareResult.DIFFERENT;
                    otherScene.objects[current.Key].result = DeserializedObject.CompareResult.DIFFERENT;
                }
            }
            else
            {
                current.Value.result = DeserializedObject.CompareResult.NEW;
            }
        }

        foreach (KeyValuePair<long, DeserializedObject> current in otherScene.objects)
        {
            if (current.Value.result == DeserializedObject.CompareResult.UNDECIDED)
            {
                current.Value.result = DeserializedObject.CompareResult.NEW;
            }
        }
    }

    public static void compare(DeserializedScene A, DeserializedScene B)
    {

    }

    public void parse()
    {
        while (index < allSceneLines.Count)
        {
            Debug.Log("before new Object " + index);

            DeserializedObject tmpO = new DeserializedObject();

            tmpO.parse(allSceneLines, ref index);

            objects.Add(tmpO.id, tmpO);

            Debug.Log("after new Object " + index);
        }
    }

    public void print(StreamWriter fout, bool withTags, bool withIDs)
    {
        fout.WriteLine(allSceneLines[0]);
        fout.WriteLine(allSceneLines[1]);

        foreach (KeyValuePair<long, DeserializedObject> current in objects)
        {
            current.Value.print(fout, withTags, withIDs);
        }
    }
}

public class DeserializedObject
{
    public long id;

    public int type;

    public string typeName;

    public List<Member> members;

    public List<int> differences;

    public enum CompareResult { UNDECIDED, EQUAL, DIFFERENT, NEW };
    public CompareResult result;

    public DeserializedObject()
    {
        members = new List<Member>();
        differences = new List<int>();

        result = CompareResult.UNDECIDED;
    }

    public bool equal(DeserializedObject other)
    {
        bool equal = true;

        for (int i = 0; i < members.Count; i++)
        {
            if (!members[i].equal(other.members[i]))
            {
                equal = false;
                differences.Add(i);
                other.differences.Add(i);
            }
        }

        return equal;
    }

    public void parse(List<string> allSceneLines, ref int index)
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
            if (line.EndsWith(":") || line.EndsWith("[]"))
            {
                Array tmpA = new Array(line.Split(':')[0]);

                tmpA.parse(allSceneLines, ref index);

                members.Add(tmpA);
            }
            else
            {
                Variable tmpV = new Variable();

                tmpV.parse(allSceneLines, ref index);

                members.Add(tmpV);
            }

            index++;
        }
    }

    public void print(StreamWriter fout, bool withTags, bool withIDs)
    {
        fout.WriteLine("--- !u!" + type + " &" + id);
        fout.WriteLine(typeName + ":");

        foreach (Member current in members)
        {
            current.print(fout, withTags, withIDs);
        }
    }
}

public abstract class Member
{
    public string name;

    public abstract bool equal(Member other);

    public abstract void parse(List<string> allSceneLines, ref int index);

    public abstract void print(StreamWriter fout, bool withTags, bool withIDs);
}

public class Variable : Member
{
    public string value;

    public long valueIsThisID;

    public override bool equal(Member m)
    {
        Variable other = (Variable)m;

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

                Debug.Log("short form " + name);

                valueIsThisID = extractID();

                return;
            }
        }

        char[] sep2 = { ':' };

        string[] pieces2 = line.Split(sep2, 2);

        this.name = pieces2[0];
        this.value = pieces2[1];

        valueIsThisID = extractID();
    }

    public override void print(StreamWriter fout, bool withTags, bool withIDs)
    {
        string lineToWrite = "";

        if (withTags)
        {
            lineToWrite += "Variable:: ";
        }

        lineToWrite += name;

        if (! name.TrimStart().Equals("- "))
        {
            lineToWrite += ":";
        }

        lineToWrite += value;

        if (withIDs)
        {
            if (valueIsThisID != -1)
            {
                lineToWrite += " " + valueIsThisID;
            }
        }

        fout.WriteLine(lineToWrite);
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

public class Array : Member
{
    public List<Variable> variables;

    public List<long> varsHaveTheseIDs;

    public Array(string n)
    {
        name = n;
        variables = new List<Variable>();
        varsHaveTheseIDs = new List<long>();        
    }

    public override bool equal(Member m)
    {
        Array other = (Array)m;

        if (string.Compare(name, other.name) != 0)
        {
            return false;
        }

        if (variables.Count != other.variables.Count)
        {
            return false;
        }

        for (int i = 0; i < variables.Count; i++)
        {
            if (!variables[i].equal(other.variables[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override void parse(List<string> allSceneLines, ref int index)
    {
        string line = allSceneLines[index];

        int offset = Array.countWhitespaces(line);

        index++;

        while (offset == Array.countWhitespaces(allSceneLines[index]) - 1)
        {
            Variable tmp = new Variable();

            tmp.parse(allSceneLines, ref index);

            variables.Add(tmp);

            index++;
        }

        index--;

        foreach (Variable current in variables)
        {
            if (current.valueIsThisID >= 0)
            {
                varsHaveTheseIDs.Add(current.valueIsThisID);
            }
        }
    }

    public override void print(StreamWriter fout, bool withTags, bool withIDs)
    {
        string lineToWrite = "";

        if (withTags)
        {
            lineToWrite += "Array   :: ";
        }

        lineToWrite += name + ":";

        if (variables.Count == 0)
        {
            lineToWrite += " []";
        }

        if (withIDs)
        {
            foreach (long current in varsHaveTheseIDs)
            {
                if (current != -1)
                {
                    lineToWrite += " " + current;
                }
            }
        }

        fout.WriteLine(lineToWrite);

        foreach (Variable current in variables)
        {
            current.print(fout, withTags, withIDs);
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
