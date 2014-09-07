using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ChangeView
{
    public SceneManager sceneManager;
    public ChangeManager changeManager;
    public Change change;
    
    public Color prevColor;
    public bool expanded;

    public ChangeView(SceneManager sceneMgr, ChangeManager chMgr, Change ch)
    {
        this.sceneManager = sceneMgr;
        this.changeManager = chMgr;
        this.change = ch;
    }

    public void onGUI(Rect editorWindow, List<Change> deleteAfter)
    {
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));

        GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));

        expanded = EditorGUILayout.Foldout(expanded, "Change " + change.nr);

        if(GUILayout.Button("Select GameObjects"))
        {
            HashSet<long> goIDs = new HashSet<long>();

            foreach (long ID in change.allIDs)
            {
                DComponent tmp;

                if (sceneManager.sceneA.components.TryGetValue(ID, out tmp))
                {
                    goIDs.Add(tmp.gameObjectReference);
                }

                tmp = null;

                if (sceneManager.sceneB.components.TryGetValue(ID, out tmp))
                {
                    goIDs.Add(tmp.gameObjectReference);
                }
            }

            GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> toBeSelected = new List<GameObject>();

            foreach (GameObject currentGO in allGOs)
            {
                long id = long.Parse( currentGO.name.Split(SceneManager.SEPSYM)[1] );

                if (goIDs.Contains(id))
                {
                    toBeSelected.Add(currentGO);
                }
            }

            Selection.objects = toBeSelected.ToArray();
        }

        if (GUILayout.Button("<"))
        {
            change.setSelected(-1);

            sceneManager.printMerged(false);

            openSceneKeepSelection(SceneManager.mergedScenePath);

            SceneView.RepaintAll();
        }

        if (GUILayout.Button(">"))
        {
            change.setSelected(1);

            sceneManager.printMerged(false);

            openSceneKeepSelection(SceneManager.mergedScenePath);

            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Save"))
        {
            deleteAfter.Add(change);
        }

        GUILayout.EndHorizontal();

        if (expanded)
        {
            foreach (long currentID in change.allIDs)
            {
                changeColor(new Color(0, 0, 0));
                GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
                resetColorToPrevious();

                changeColorOnCondition(change.selected < 0 ? true : false, Color.green, Color.red);
                GUILayout.BeginVertical(GUILayout.MaxWidth(editorWindow.width / 2));

                if (sceneManager.sceneA.components.ContainsKey(currentID))
                {
                    DComponent tmp = sceneManager.sceneA.components[currentID];
                    
                    tmp.onGUI();
                }
                else
                {
                    GUILayout.Label("");
                }

                GUILayout.EndVertical();
                resetColorToPrevious();

                changeColorOnCondition(change.selected > 0 ? true : false, Color.green, Color.red);
                GUILayout.BeginVertical(GUILayout.MaxWidth(editorWindow.width / 2));

                if (sceneManager.sceneB.components.ContainsKey(currentID))
                {
                    DComponent tmp = sceneManager.sceneB.components[currentID];

                    tmp.onGUI();
                }
                else
                {
                    GUILayout.Label("");
                }

                GUILayout.EndVertical();
                resetColorToPrevious();

                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    private void changeColorOnCondition(bool condition, Color trueCase, Color falseCase)
    {
        if (condition)
        {
            changeColor(trueCase);
        }
        else
        {
            changeColor(falseCase);
        }
    }

    private void changeColor(Color desiredColor)
    {
        prevColor = GUI.color;
        GUI.color = desiredColor;
    }

    private void resetColorToPrevious()
    {
        GUI.color = prevColor;
    }

    private void openSceneKeepSelection(string path)
    {
        string selectedName = "";
        bool somethingWasSelected = false;

        if (Selection.activeGameObject != null)
        {
            selectedName = Selection.activeGameObject.name;
            somethingWasSelected = true;
        }

        EditorApplication.OpenScene(path);

        if (somethingWasSelected)
        {
            Selection.activeGameObject = GameObject.Find(selectedName);
        }
    }

    private void showSelectButton(long currentID)
    {
        if (GUILayout.Button("Select GO"))
        {
            GameObject[] allGOs = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject currentGO in allGOs)
            {
                if (currentGO.name.EndsWith("" + currentID))
                {
                    Selection.activeGameObject = currentGO;
                }
            }
        }
    }
}
