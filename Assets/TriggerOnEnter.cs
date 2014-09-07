using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerOnEnter : MonoBehaviour 
{
    public List<Triggerable> toTrigger;
    public bool triggerOnce;
    public bool triggered;
	

    void OnTriggerEnter()
    {
        if (!triggerOnce || !triggered)
        {

            foreach (Triggerable current in toTrigger)
            {
                current.trigger();
            }
            triggered = true;
        }
    }
}
