using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class Deserialized
{
    public string name;

    public abstract bool isEqualTo(Deserialized other);

    public abstract void parse(List<string> allSceneLines, ref int index);

    public abstract List<long> getReferencesOfDiffs();

    public abstract void print(StreamWriter fout, bool merged, bool debug);

    public abstract void onGUI();
}


