using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlastDoorMaster : Triggerable 
{
    public BlastDoor[] doors;

    public float delay;

    public bool triggerOnCollision;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public override void trigger()
    {
        if (!(triggerOnce && triggered))
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].steps = 1 + delay * i;
                doors[i].trigger();
            }

            triggered = true;
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (triggerOnCollision && c.tag.Equals("Player"))
        {
            this.trigger();
        }
    }
}
