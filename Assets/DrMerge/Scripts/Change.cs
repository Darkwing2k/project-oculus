using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Change
{
    public static List<long> allInvolvedObjectsA = new List<long>();
    public static List<long> allInvolvedObjectsB = new List<long>();

    public List<long> involvedObjectsA;
    public List<long> involvedObjectsB;

    public Change()
    {
        involvedObjectsA = new List<long>();
        involvedObjectsB = new List<long>();
    }

    public void addInvolvedObjectA(long id)
    {
        involvedObjectsA.Add(id);
        allInvolvedObjectsA.Add(id);
    }

    public void addInvolvedObjectB(long id)
    {
        involvedObjectsB.Add(id);
        allInvolvedObjectsB.Add(id);
    }
}
