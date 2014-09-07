using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlastDoorMaster : Triggerable, Resettable
{
    public BlastDoor[] doors;

    public float delay;

    public bool triggerOnCollision;

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

	public void SetReferencesOnRespawn()
	{
		for(int i=0; i<doors.Length; i++) {
			GameObject neededReference = GameObject.Find ("/" + doors[i].name);
			doors[i] = neededReference.GetComponent<BlastDoor>();
		}
	}
}
