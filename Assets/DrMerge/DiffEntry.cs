using UnityEngine;
using UnityEditor;
using System.Collections;

public class DiffEntry : Entry
{
    private GoEntryDiff parent;

    public GoEntryDiff Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    private string lineA;
    private string lineB;

    public DiffEntry(DiffDeciderModel m, int indexS, int indexE, string lineA, string lineB)
        : base(m, indexS, indexE)
    {
        model = m;

        this.lineA = lineA;
        this.lineB = lineB;
    }

    public override void onGUI()
    {
        EditorGUILayout.BeginHorizontal();
        
        Color prev = GUI.color;

        if (parent.ChosenGo < 0)
        {
            GUI.color = Color.green;
            EditorGUILayout.LabelField(lineA);

            GUI.color = Color.red;
            EditorGUILayout.LabelField(lineB);
        }
        else if (parent.ChosenGo > 0)
        {
            GUI.color = Color.red;
            EditorGUILayout.LabelField(lineA);

            GUI.color = Color.green;
            EditorGUILayout.LabelField(lineB);
        }
        else
        {
            EditorGUILayout.LabelField(lineA);
            EditorGUILayout.LabelField(lineB);
        }

        GUI.color = prev;

        /*
        if (GUILayout.Button("<"))
        {
            chosenDiff = -1;
            model.changeComment(indexStart, lineB, lineA);
        }

        if (GUILayout.Button(">"))
        {
            chosenDiff = 1;
            model.changeComment(indexStart, lineA, lineB);
        }
        */

        EditorGUILayout.EndHorizontal();
    }

    public override void updateLineNrs(int from, int offset)
    {
        if (indexStart > from)
        {
            indexStart += offset;
            indexEnd += offset;
        }
    }
}
