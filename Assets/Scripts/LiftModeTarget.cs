using UnityEngine;
using System.Collections;

public class LiftModeTarget : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            c.GetComponent<Player>().registerLiftable(this);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            c.GetComponent<Player>().removeLiftable(this);
        }
    }
}
