using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropZone : MonoBehaviour 
{

    public GameObject key;

    public Triggerable objectToTrigger;

    public bool pressed;

    private AudioSource audio;

	// Use this for initialization
	void Start () 
    {
        audio = GetComponent<AudioSource>();

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
            if (objectToTrigger != null)
            {
                objectToTrigger.trigger();
                pressed = true;
            }
            audio.Play();
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
