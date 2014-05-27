using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
    public Triggerable objectToTrigger;
    public Light lightToTrigger;

    public GameObject playerRef;

    public bool pressed;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(playerRef != null && Input.GetButtonDown("Use"))
        {
            if (!pressed)
            {
                objectToTrigger.trigger();
                lightToTrigger.enabled = true;
                pressed = true;
            }
            else
            {
                objectToTrigger.trigger();
                lightToTrigger.enabled = false;
                pressed = false;
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef = null;
        }
    }
}
