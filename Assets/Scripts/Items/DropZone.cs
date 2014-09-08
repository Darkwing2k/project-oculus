using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropZone : Triggerable 
{
    public GameObject key;

    public Triggerable objectToTrigger;

    public bool powered;

    public bool pressed;

    public bool keyPresent;

    private AudioSource audio;

	// Use this for initialization
	void Start () 
    {
        audio = GetComponent<AudioSource>();
        triggerOnce = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public override void trigger()
    {
        triggered = !triggered;
        powered = !powered;

        powerChanged();
    }

    void OnCollisionEnter(Collision c)
    {
        if (key.GetInstanceID() == c.gameObject.GetInstanceID())
        {
            keyPresent = true;

            keyPresenceChanged();
        }
    }

    void OnCollisionExit(Collision c)
    {
        if (key.GetInstanceID() == c.gameObject.GetInstanceID())
        {
            keyPresent = false;

            keyPresenceChanged();
        }
    }

    private void powerChanged()
    {
        if (pressed && !powered)
        {
            changePressTo(false);
        }

        if (keyPresent && powered)
        {
            changePressTo(true);
        }
    }

    private void keyPresenceChanged()
    {
        if (pressed && !keyPresent)
        {
            changePressTo(false);
        }

        if (keyPresent && powered)
        {
            changePressTo(true);
        }
    }

    private void changePressTo(bool newPress)
    {
        if (newPress)
        {
            if (audio != null)
            {
                audio.Play();
            }
        }

        pressed = newPress;
        objectToTrigger.trigger();
    }
}
