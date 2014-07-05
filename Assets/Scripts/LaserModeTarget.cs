using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserModeTarget : MonoBehaviour 
{
    public float duration;

    public bool isCut;

    public List<Component> destroyWhenCut;

	// Use this for initialization
	void Start () 
    {
        isCut = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnTriggerEnter(Collider c)
    {
        if (!isCut && c.gameObject.tag.Equals("Player") && !isCut)
        {
            c.GetComponent<Player>().registerCutable(this);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (!isCut && c.gameObject.tag.Equals("Player"))
        {
            c.GetComponent<Player>().removeCutable(this);
        }
    }

    public void Cut()
    {
        foreach (Component c in destroyWhenCut)
            Destroy(c);

        destroyWhenCut.Clear();

        isCut = true;
    }
}
