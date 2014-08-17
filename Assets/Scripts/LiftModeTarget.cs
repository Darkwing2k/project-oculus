using UnityEngine;
using System.Collections;

public class LiftModeTarget : MonoBehaviour
{
    public bool imAnEnergyPackage;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            Player p = c.GetComponent<Player>();

            p.registerLiftable(this);
            p.buttonInfo.gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            Player p = c.GetComponent<Player>();

            p.removeLiftable(this);
            p.buttonInfo.gameObject.SetActive(false);
        }
    }
}
