using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChangeManager
{
    public SceneManager sceneMgr;

    public Dictionary<int, Change> changes;
    public Dictionary<int, ChangeView> changeViews;

    public HashSet<long> allChangedIDs;

    public ChangeManager(SceneManager mgr, HashSet<long> allChangedIDs)
    {
        sceneMgr = mgr;

        this.allChangedIDs = allChangedIDs;

        changes = new Dictionary<int, Change>();
        changeViews = new Dictionary<int, ChangeView>();

        createChanges();

        createChangeViews();
    }

    private void createChanges()
    {
        foreach (long ID in allChangedIDs)
        {
            DComponent currentCmp;

            if (sceneMgr.sceneA.components.TryGetValue(ID, out currentCmp))
            {
                Change currentChg;

                if (changes.TryGetValue(currentCmp.changeNr, out currentChg))
                {
                    currentChg.addToA(currentCmp.id);
                }
                else
                {
                    currentChg = new Change(sceneMgr, currentCmp.changeNr);
                    currentChg.addToA(currentCmp.id);

                    changes.Add(currentCmp.changeNr, currentChg);
                }
            }

            currentCmp = null;

            if (sceneMgr.sceneB.components.TryGetValue(ID, out currentCmp))
            {
                Change currentChg;

                if (changes.TryGetValue(currentCmp.changeNr, out currentChg))
                {
                    currentChg.addToB(currentCmp.id);
                }
                else
                {
                    currentChg = new Change(sceneMgr, currentCmp.changeNr);
                    currentChg.addToB(currentCmp.id);

                    changes.Add(currentCmp.changeNr, currentChg);
                }
            }
        }

        foreach (Change currentChange in changes.Values)
        {
            currentChange.setSelected(-1);
        }
    }

    private void createChangeViews()
    {
        foreach (Change currentChange in changes.Values)
        {
            ChangeView tmp = new ChangeView(sceneMgr, this, currentChange);

            changeViews.Add(currentChange.nr, tmp);
        }
    }

    public void removeChange(Change change)
    {
        changeViews.Remove(change.nr);
        changes.Remove(change.nr);
    }
}

public class Change
{
    public int nr;

    public int selected;

    public SceneManager sceneMgr;

    public HashSet<long> allIDs;

    private HashSet<long> componentsA;
    private HashSet<long> componentsB;

    public Change(SceneManager mgr, int changeNr)
    {
        this.sceneMgr = mgr;

        nr = changeNr;

        allIDs = new HashSet<long>();
        componentsA = new HashSet<long>();
        componentsB = new HashSet<long>();
    }

    public void setSelected(int i)
    {
        selected = i;

        foreach (long ID in componentsA)
        {
            sceneMgr.sceneA.components[ID].chosen = selected < 0 ? true : false;
        }

        foreach (long ID in componentsB)
        {
            sceneMgr.sceneB.components[ID].chosen = selected > 0 ? true : false;
        }
    }

    public void addToA(long id)
    {
        componentsA.Add(id);
        allIDs.Add(id);
    }

    public void addToB(long id)
    {
        componentsB.Add(id);
        allIDs.Add(id);
    }
}
