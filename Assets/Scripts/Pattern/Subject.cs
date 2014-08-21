using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Subject : MonoBehaviour
{
    private List<Observer> observer;

    protected void notify()
    {
        foreach (Observer current in observer)
        {
            current.onNotify();
        }
    }

    public void addObserver(Observer o)
    {
        observer.Add(o);
    }

    public void removeObserver(Observer o)
    {
        observer.Remove(o);
    }
}
