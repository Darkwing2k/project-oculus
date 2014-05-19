using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropZone : MonoBehaviour 
{

    public List<GameObject> triggerObjects;

    public int amountTillTrigger;

    private List<GameObject> presentObjects;

	// Use this for initialization
	void Start () 
    {
        //triggerObjects = new List<GameObject>();
        presentObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnCollisionEnter(Collision c)
    {
        if (triggerObjects.Contains(c.gameObject))
        {
            presentObjects.Add(c.gameObject);

            if (presentObjects.Count >= amountTillTrigger)
            {
                print("DropZone triggered");
            }
        }
    }

    void OnCollisionExit(Collision c)
    {
        if (presentObjects.Contains(c.gameObject))
        {
            presentObjects.Remove(c.gameObject);
        }
    }
}
