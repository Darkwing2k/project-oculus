using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GoEntryDiff : GoEntry
{
    private bool show;

    private int commentStart;

    private int indexName2;

    private List<DiffEntry> diffs;

    private short chosenGo;

    public short ChosenGo
    {
        get { return chosenGo; }
    }

    private bool decided;

    public GoEntryDiff(DiffDeciderModel m, List<DiffEntry> diffs, string cName, int indexS, int indexC, int indexE, int indexN, int indexN2)
        : base(m, cName, indexS, indexE, indexN)
    {
        model = m;

        show = false;

        this.diffs = diffs;

        foreach (DiffEntry current in this.diffs)
        {
            current.Parent = this;
        }

        commentStart = indexC;

        indexName2 = indexN2;

        Debug.Log(indexStart);
        Debug.Log(commentStart);
        Debug.Log(indexEnd);

        Debug.Log(indexName);
        Debug.Log(indexName2);
    }

    public override void onGUI()
    {
        EditorGUILayout.BeginVertical();

        this.show = EditorGUILayout.Foldout(show, originalName + " " + indexStart.ToString());
        if (this.show)
        {
            EditorGUILayout.BeginHorizontal();

            //bool decided = true;

            /*
            for (int i = 0; i < diffs.Count; i++)
            {
                if (diffs[i].ChosenDiff == 0)
                    decided = false;
            }
             * */

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(SceneMerger.a.name))
            {
                if (chosenGo != -1)
                {
                    chosenGo = -1;

                    if (decided)
                        model.changeComment(indexStart, commentStart, commentStart, indexEnd);

                    decided = true;
                }
            }

            if (GUILayout.Button(SceneMerger.b.name))
            {
                if (chosenGo != 1)
                {
                    chosenGo = 1;

                    model.changeComment(commentStart, indexEnd, indexStart, commentStart);

                    decided = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < diffs.Count; i++)
            {
                diffs[i].onGUI();
            }

            EditorGUILayout.EndVertical();

            if (decided)
            {
                if (GUILayout.Button("Apply"))
                {
                    if (chosenGo < 0)
                    {
                        model.changeName(indexName, originalName, false);
                        model.removeLines(commentStart, indexEnd - 1);
                    }
                    else
                    {
                        model.changeName(indexName2, originalName, false);
                        model.removeLines(indexStart, commentStart - 1);
                    }

                    model.removeMe(this);
                }
            }
        }

        EditorGUILayout.EndVertical();
    }
}
