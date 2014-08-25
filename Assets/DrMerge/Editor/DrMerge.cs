using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class DrMerge : EditorWindow {

    private bool performedMerge = false;
    private bool merging = false;

    private int counter;
    private int counterMaxForRepaint = 10;
    private float mergingProgress;

    private bool showNewGo;
    private bool showDiffGo;
    private bool[] showDifferences;
    private short[] selectedDiff;

    private DiffDeciderModel ddModel;

    [MenuItem("Window/DrMerge")]
    static void Init()
    {
        DrMerge window = (DrMerge)EditorWindow.GetWindow<DrMerge>("DrMerge");
        window.Show();
        
    }

    private void OnLostFocus()
    {
        /*
        performedMerge = false;
        merging = false;
        mergingProgress = 0;
        counter = 0;
        */
    }

    private void OnDestroy()
    {
        Debug.Log("DrMerge destroyed");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reset"))
        {
            performedMerge = false;
            merging = false;
            //newEntries.Clear();
            mergingProgress = 0;
            counter = 0;
        }

        if (!performedMerge)
        {
            #region first UI screen
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Scene A");
            SceneMerger.a.original = EditorGUILayout.ObjectField(SceneMerger.a.original, typeof(Object), true);

            GUILayout.Label("Scene B");
            SceneMerger.b.original = EditorGUILayout.ObjectField(SceneMerger.b.original, typeof(Object), true);


            if (GUILayout.Button("Start"))
            {
                merging = true;

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = false;

                worker.DoWork += (sender, e) => { SceneMerger.runComparison(); };
                worker.RunWorkerCompleted += (sender, e) => { Debug.Log("feddich!"); performedMerge = true; };

                SceneMerger.getPathsAndNames();

                SceneMerger.activateProgressReports(fetchMergeProgress);

                //worker.RunWorkerAsync();
            }

            if (merging)
            {
                EditorGUI.ProgressBar(new Rect(5, 100, this.position.width - 10, 20), mergingProgress, "Merging");
            }

            EditorGUILayout.EndVertical();
            #endregion
        }

        if (performedMerge && merging)
        {
            Debug.Log("Merge done, Initialising DiffDecider");

            AssetDatabase.Refresh();

            EditorApplication.OpenScene(DrMergeUtil.relativeMergedScenePath);

            //DiffDecider.fetchAllSceneLines();

            //DiffDecider.fetchEntries();

            //DiffDecider.fetchDifferences();

            ddModel = new DiffDeciderModel();

            //showDifferences = new bool[DiffDecider.diffGos.Count];
            //selectedDiff = new short[DiffDecider.diffGos.Count];

            merging = false;
        }

        if(performedMerge && !merging)
        {
            EditorGUILayout.BeginVertical();

            showNewGo = EditorGUILayout.Foldout(showNewGo, "New GameObjects (" + ddModel.NewEntriesCount + ")");
            if (showNewGo)
            {
                ddModel.newGoOnGUI();
            }

            showDiffGo = showDiffGo = EditorGUILayout.Foldout(showDiffGo, "Different GameObjects (" + ddModel.DiffEntriesCount + ")");
            if (showDiffGo)
            {
                ddModel.diffGoOnGUI();
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void Update()
    {
        if (merging)
        {
            if (counter > counterMaxForRepaint)
            {
                this.Repaint();
            }
        }
    }

    public void fetchMergeProgress(float percent)
    {
        mergingProgress = percent;

        counter++;
    }
}
