using UnityEngine;
using System.Collections;

public abstract class GoEntry : Entry
{
    protected DiffDeciderModel model;

    protected string currentName;
    protected string originalName;

    protected int indexName;

    public GoEntry(DiffDeciderModel m, string cName, int indexS, int indexE, int indexN)
        : base (m, indexS, indexE)
    {
        currentName = cName;
        originalName = cName.Split(DrMergeUtil.SEPSYM)[0];
        indexName = indexN;
    }

    public override void updateLineNrs(int from, int offset)
    {
        if (indexStart > from)
        {
            indexStart += offset;
            indexEnd += offset;
            indexName += offset;

            string[] pieces = currentName.Split(DrMergeUtil.SEPSYM);

            pieces[pieces.Length - 2] = (indexStart + 1).ToString();

            currentName = originalName;

            for (int i = 1; i < pieces.Length; i++)
            {
                currentName += DrMergeUtil.SEPSYM + pieces[i];
            }

            model.changeName(indexName, currentName, false);
        }
    }
}
