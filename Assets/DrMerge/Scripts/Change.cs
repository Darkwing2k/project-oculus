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
}
