using UnityEngine;
using System.Collections;
using System.IO;

public class LineCountWriter : StreamWriter 
{
    public long lineCount;

    public LineCountWriter(string path) : base(path)
    {
        lineCount = 0;
    }

    public override void WriteLine()
    {
        lineCount++;

        base.WriteLine();
    }

    /*
    public void proceedToLine(long lineNr)
    {
        while (lineCount < lineNr)
        {
            this.WriteLine("");
        }
    }
    */
}
