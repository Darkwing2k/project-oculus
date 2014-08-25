using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GoEntryNew : GoEntry
{
    public GoEntryNew(DiffDeciderModel m, string cName, int indexS, int indexE, int indexN)
        : base(m, cName, indexS, indexE, indexN)
    {
        model = m;
    }

    public override void onGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField(originalName + " " + indexStart.ToString());

        if (GUILayout.Button("Select"))
        {
            GameObject go = GameObject.Find(currentName);

            Selection.activeGameObject = go;
            Selection.activeTransform = go.transform;
        }

        if (GUILayout.Button("Keep"))
        {
            model.changeName(indexName, originalName, true);
            model.removeMe(this);
        }

        if (GUILayout.Button("Delete"))
        {
            model.removeLines(indexStart, indexEnd);
            model.removeMe(this);
        }

        EditorGUILayout.EndHorizontal();
    }
}
