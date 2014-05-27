using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropZone : MonoBehaviour 
{

    public GameObject key;

    public Triggerable objectToTrigger;

    public bool pressed;

	// Use this for initialization
	void Start () 
    {
        if (key == null)
        {
            print("DropZone " + gameObject.name + " has no key!");
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnCollisionEnter(Collision c)
    {
        if (key.GetInstanceID() == c.gameObject.GetInstanceID() && !pressed)
        {
            objectToTrigger.trigger();
            pressed = true;
        }
    }

    void OnCollisionExit(Collision c)
    {
        if (key.GetInstanceID() == c.gameObject.GetInstanceID() && pressed)
        {
            objectToTrigger.trigger();
            pressed = false;
        }
    }
}
