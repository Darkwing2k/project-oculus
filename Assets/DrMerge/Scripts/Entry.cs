using UnityEngine;
using System.Collections;

public abstract class Entry
{
    protected DiffDeciderModel model;

    protected int indexStart;
    protected int indexEnd;

    public Entry(DiffDeciderModel m, int indexS, int indexE)
    {
        this.model = m;
        indexStart = indexS;
        indexEnd = indexE;
    }

    public abstract void onGUI();

    public abstract void updateLineNrs(int from, int offset);
}