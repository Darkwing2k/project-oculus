using UnityEngine;
using System.Collections;
using System.IO;

public class LineCountReader :  StreamReader
{
    public long lineCount;
    public string path;

    public LineCountReader(string path) : base(path)
    {
        lineCount = 0;
        this.path = path;
    }

    public override string ReadLine()
    {
        lineCount++;

        return base.ReadLine();
    }

    public void proceedToLine(long lineNr)
    {
        while (lineCount < lineNr)
        {
            this.ReadLine();
        }
    }
}
