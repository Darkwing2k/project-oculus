using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DrMergeParser 
{
    public DeserializedScene sceneA;
    public DeserializedScene sceneB;

    public DrMergeParser(string pathToA, string pathToB)
    {
        List<string> allLines = new List<string>();

        using(StreamReader fin = new StreamReader(pathToA))
        {
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                allLines.Add(line);
            }
        }

        sceneA = new DeserializedScene(allLines);

        allLines = new List<string>();

        using (StreamReader fin = new StreamReader(pathToB))
        {
            string line = null;

            while ((line = fin.ReadLine()) != null)
            {
                allLines.Add(line);
            }
        }

        sceneB = new DeserializedScene(allLines);
    }

    public void parse()
    {
        sceneA.parse();
        sceneB.parse();
    }

    public void print(string path, bool withTags, bool withIDs)
    {
        using (StreamWriter fout = new StreamWriter(path, false))
        {
            sceneA.print(fout, withTags, withIDs);

            fout.WriteLine();
            fout.WriteLine(".+*´*+..+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.+*´*+.");
            fout.WriteLine();

            sceneB.print(fout, withTags, withIDs);
        }
    }
}


