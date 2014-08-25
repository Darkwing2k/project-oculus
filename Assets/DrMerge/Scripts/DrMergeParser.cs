using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DrMergeParser 
{
    
}

public class DeserializedScene
{
    public string name;

    public Dictionary<long, DeserializedObject> objects;

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
}

public class DeserializedObject
{
    public long id;

    public int type;

    public string typeName;

    public List<string> allLines;

    public List<Member> members;

    public List<int> differences;

    public enum CompareResult { UNDECIDED, EQUAL, DIFFERENT, NEW };
    public CompareResult result;

    public DeserializedObject()
    {
        allLines = new List<string>();
        members = new List<Member>();
        differences = new List<int>();

        result = CompareResult.UNDECIDED;
    }

    public DeserializedObject(long id, int type, string typeName) : this()
    {
        this.id = id;
        this.type = type;
        this.typeName = typeName;
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
}

public abstract class Member
{
    public string name;

    public abstract bool equal(Member other);

    public abstract void parse(List<string> allSceneLines, int index);
}

public class Variable : Member
{
    public string value;

    public override bool equal(Member m)
    {
        Variable other = (Variable)m;

        if (string.Compare(name, other.name) != 0)
        {
            return false;
        }

        if(string.Compare(value, other.value) != 0)
        {
            return false;
        }

        return true;
    }

    public override void parse(List<string> allSceneLines, int index)
    {
        string line = allSceneLines[index];

        if (line.Contains("{"))
        {
            if (line.IndexOf('{') < line.IndexOf(':'))
            {
                char[] sep = { '{' };

                string[] pieces = line.Split(sep, 2);

                this.name = pieces[0];
                this.value = pieces[1];

                return;
            }
        }

        char[] sep2 = { ':' };

        string[] pieces2 = line.Split(sep2, 2);

        this.name = pieces2[0];
        this.value = pieces2[1];
    }
}

public class Array : Member
{
    public List<Variable> variables;

    public Array(string n)
    {
        name = n;
        variables = new List<Variable>();
    }

    public override bool equal(Member m)
    {
        Array other = (Array) m;

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

    public override void parse(List<string> allSceneLines, int index)
    {
        string line = allSceneLines[index];

        this.name = line.Split(':')[0];

        int i = 1;

        while (allSceneLines[index + i].Trim().StartsWith("- "))
        {
            Variable tmp = new Variable();

            tmp.parse(allSceneLines, index + i);

            variables.Add(tmp);

            i++;
        }
    }
}
