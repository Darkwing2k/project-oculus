using UnityEngine;
using System.Collections;

public class LaserMode : MonoBehaviour 
{

    public BoxCollider trigger;

    private GameObject cutable;
    public bool cutMode = false;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Use"))
        {
            if (!cutMode && cutable != null)
            {
                cutMode = true;
                this.transform.parent.GetComponent<FlightControl>().controlsActivated = false;

                
            }
            else if(cutMode)
            {
                cutMode = false;
                this.transform.parent.GetComponent<FlightControl>().controlsActivated = true;
            }
        }

        if (cutMode)
        {
            Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag.Equals("Cutable"))
                {
                    print("target acquired");
                }
            }

            Debug.DrawRay(r.origin, r.direction, Color.cyan);
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Cutable")
        {
            print("cutable object in range");
            cutable = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Cutable")
        {
            print("cutable object left range");
            cutable = null;
        }
    }
}
