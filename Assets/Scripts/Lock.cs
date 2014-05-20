using UnityEngine;
using System.Collections;

public class Lock : MonoBehaviour 
{
    public float duration;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            print("player entered trigger of cutable object");
            c.gameObject.GetComponentInChildren<LaserMode>().cutable.Add(this.gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            print("player left trigger of cutable object");
            c.gameObject.GetComponentInChildren<LaserMode>().cutable.Remove(this.gameObject);
        }
    }
}
