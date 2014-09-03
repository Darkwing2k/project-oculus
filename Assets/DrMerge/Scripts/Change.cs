using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Change
{
    public static int changeCount = 0;

    DeserializedScene scene;

    public int nr;
    public List<long> references;
    public Change partner;

    public bool chosen;
    public bool expanded;

    public Change(DeserializedScene s, bool chosen)
    {
        this.chosen = chosen;
        scene = s;

        nr = changeCount;
        changeCount++;

        references = new List<long>();
    }

    public void setChosen(bool value)
    {
        chosen = value;
        updateChosenInRefs();

        partner.chosen = !value;
        partner.updateChosenInRefs();
    }

    public void updateChosenInRefs()
    {
        foreach (long ID in references)
        {
            scene.objects[ID].chosen = this.chosen;
        }
    }

    public void createPartnerIfNull()
    {
        if (partner == null)
        {
            Debug.Log(nr + " here. My partner was null!");

            partner = new Change(scene.opponent, !chosen);

            scene.opponent.changes.Add(partner);
        }
    }
}


public class Change2
{
    public SceneManager manager;

    public Dictionary<long, DObject> objectsA;
    public Dictionary<long, DObject> objectsB;

    public int selected;
    public bool expanded;

    public static int changeCount = 0;
    public int nr;

    public Change2(SceneManager manager)
    {
        nr = changeCount;
        changeCount++;

        this.manager = manager;

        objectsA = new Dictionary<long, DObject>();
        objectsB = new Dictionary<long, DObject>();
    }
}
