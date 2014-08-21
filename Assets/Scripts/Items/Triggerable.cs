using UnityEngine;
using System.Collections;

public abstract class Triggerable : MonoBehaviour
{
    public bool triggerOnce;
    public bool triggered;

    public abstract void trigger();
}
