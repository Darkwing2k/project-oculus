using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Button : MonoBehaviour 
{
    public List<Triggerable> objectsToTrigger;

    public GameObject playerRef;

    public bool pressed;
    public bool toggle;

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
                foreach(Triggerable t in objectsToTrigger)
                    t.trigger();

                pressed = true;
            }
            else if(pressed && toggle)
            {
                foreach (Triggerable t in objectsToTrigger)
                    t.trigger();

                pressed = false;
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef = c.gameObject;

            playerRef.GetComponent<Player>().buttonInfo.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef.GetComponent<Player>().buttonInfo.gameObject.SetActive(false);

            playerRef = null;
        }
    }
}
