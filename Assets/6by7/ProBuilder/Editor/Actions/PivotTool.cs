/**
 *  @ Matt1988
 *  This extension was built by @Matt1988
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class PivotTool : Editor {

    [MenuItem("Window/ProBuilder/Actions/Set Pivot _%j")]
    static void init()
    {
        pb_Editor_Utility.ShowNotification("Set Pivot", "Center pivot around current selection.");

        pb_Object[] pbObjects = GetSelection();
        if (pbObjects.Length > 0)
        {
            Undo.RegisterUndo(EditorUtility.CollectDeepHierarchy(Selection.transforms as Object[]), "set object(s) pivot point.");

            foreach (pb_Object pbo in pbObjects)
            {
                if (pbo.selected_triangles.Count > 0)
                {
                    SetPivot(pbo, pbo.selected_triangles.ToArray(), false);
                }
                else
                {
                    SetPivot(pbo, pbo.uniqueIndices, true);
                }
            }
        }
    }
   
    static pb_Object[] GetSelection()
    {
        return pbUtil.GetComponents<pb_Object>(Selection.transforms);       
    }

    private static void SetPivot(pb_Object pbo, int[] testIndices, bool doSnap)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 vector in pbo.VerticesInWorldSpace(testIndices))
        {
            center += vector;
        }
        center /= testIndices.Length;
            
        if(doSnap)
            center = pbUtil.SnapValue(center, Vector3.one, SixBySeven.Shared.snapValue);

        Vector3 dir = (pbo.transform.position - center);

        pbo.transform.position = center;

        // the last bool param force disables snapping vertices
        pbo.TranslateVertices(pbo.uniqueIndices, dir, true);
    }
}
